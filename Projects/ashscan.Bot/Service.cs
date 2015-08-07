﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ChatSharp;
using ChatSharp.Events;

namespace Ashscan.Bot
{
    using ExploitFinder;

    using Extensibility;

    using Mono.Addins;

    using Properties;
using System.Text.RegularExpressions;

    public class Service
    {
        private IEnumerable<string> ignoredChannels;

        private IEnumerable<string> watchedChannels;

        private IEnumerable<string> botOperators;

        internal static IrcClient ircClient;

        private CancellationTokenSource cancellationTokenSource;

        internal static ConcurrentQueue<string> rawMessageQueue;

        private IConnectableObservable<string> raw;

        private IDisposable rawHolder;
        private AggregateHostExploitFinder exploitFinders;

        private Dictionary<int, IEnumerable<Exploit>> untoleratedExploits;
        
        public Service()
            : base()
        {

            this.ignoredChannels = new[]
            {
               ConfigHelper.Config.ReportingChannel
            };
            this.watchedChannels = ConfigHelper.Config
                        .WatchedChannels
                     .Split(',').Where(c => c.StartsWith("#"));
            this.botOperators =  ConfigHelper.Config
                                .BotOperators.Split(',')
                                .Select(o => 
                                {
                                    var s = o.Trim();

                                    if (!s.Contains("!"))
                                    {
                                        s = string.Format("{0}!*@*", s);
                                    }

                                    return WildCardToRegular(s);
                                });

            CreateIrcClient();

            untoleratedExploits = this.GetUntoleratedExploitTypes();            
        }

        private void CreateIrcClient()
        {
            ircClient = new IrcClient(ConfigHelper.Config.Network, new IrcUser(ConfigHelper.Config.Nick, ConfigHelper.Config.Username, string.Empty, ConfigHelper.Config.Fullname));
            
            var blackListedHostsPath = ConfigHelper.Config.BlackListedHostsPath;
            var abusiveWordsPath = ConfigHelper.Config.AbusiveWordsPath;
            // NOTE : do these objects have to be recreated between reconnects? -- Diabolic 15/03/2015
            this.cancellationTokenSource = new CancellationTokenSource();
            rawMessageQueue = new ConcurrentQueue<string>();
            this.raw = Observable.Create<string>
            (
                (o, t) => Task.Factory.StartNew
                (
                    () =>
                    {
                        while (!t.IsCancellationRequested)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1.0));

                            var message = string.Empty;

                            if (rawMessageQueue.TryDequeue(out message) && !string.IsNullOrWhiteSpace(message))
                            {
                                o.OnNext(message);
                            }
                        }
                    },
                    t)).Publish();
            this.raw.ObserveOn(Scheduler.Default).Subscribe((message) => ircClient.SendRawMessage(message));
            this.rawHolder = this.raw.Connect();
            this.exploitFinders = new AggregateHostExploitFinder(
                abusiveWordsPath,
                () =>
                ircClient.Channels.SelectMany(
                    c => c.UsersByMode.ContainsKey('v') ? c.UsersByMode['v'] : Enumerable.Empty<IrcUser>())
                    .Select(a => a.Nick)
                    .ToList(),
                blackListedHostsPath,
                ConfigHelper.Config.LongNickLength);

            ircClient.ConnectionComplete += HandleClientConnectionCompleteEvent;
            ircClient.RawMessageRecieved += HandleRawMessageReceived;
            ircClient.UserMessageRecieved += HandleUserMessageReceived;
            ircClient.UserJoinedChannel += HandleUserJoinedChannel;
            ircClient.NetworkError += ircClient_NetworkError;
        }

        public void Start()
        {
            ircClient.ConnectAsync();
        }

        public void Stop()
        {
            rawHolder.Dispose();
            ircClient.Quit();
        }

        private void HandleClientConnectionCompleteEvent(object sender, EventArgs e)
        {
            var client = sender as IrcClient;

            if (client != null)
            {
                client.JoinChannel(ConfigHelper.Config.ReportingChannel);

                client.SendRawMessage(":MODE {0} +iRCB-ws", ConfigHelper.Config.Nick);
                client.SendRawMessage("NickServ IDENTIFY " + ConfigHelper.Config.NickservPassword);

                // TODO : remove the Thread.Sleep; replace with asynchronous code in an appropriate event -- Diabolic 15/03/2015
                Thread.Sleep(100);

                this.watchedChannels.ToList().ForEach(client.JoinChannel);
            }
        }


        void ircClient_NetworkError(object sender, SocketErrorEventArgs e)
        {
            var config = ConfigHelper.Config;

            Console.WriteLine("Network error: Will reconnect=" + config.AutoReconnect);

            if (config.AutoReconnect)
            {
                var seconds = config.AutoReconnectTimer;
                ircClient = new IrcClient(ConfigHelper.Config.Network, new IrcUser(ConfigHelper.Config.Nick, ConfigHelper.Config.Username, string.Empty, ConfigHelper.Config.Fullname));
                Thread.Sleep(TimeSpan.FromSeconds(seconds));


                CreateIrcClient();

                ircClient.ConnectAsync();
            }
        }
        private void HandleRawMessageReceived(object sender, RawMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
        private string WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }

        private void HandleUserMessageReceived(object sender, PrivateMessageEventArgs e)
        {
            var client = sender as IrcClient;

            if (client != null)
            {
                try
                {
                    if (e.PrivateMessage != null)
                    {
                        var split = e.PrivateMessage.Message.Split(' ');
                        var user = e.PrivateMessage.User;
                        var command = ExtensionManager.GetCommandHandler(split[0]);

                        if (command != null && (!command.OperatorsOnly || this.botOperators.Any(x => Regex.IsMatch(string.Format("{0}!{1}@{2}", user.Nick, user.User, user.Hostname), x))))
                        {
                            command.Handle(new UserInfo(e.PrivateMessage.User),  split);
                        }
                        else
                        {
                            rawMessageQueue.Enqueue(
                                   string.Format(
                                       "PRIVMSG {0} :Message from {1}: {2}",
                                       ConfigHelper.Config.ReportingChannel,
                                        e.PrivateMessage.User.Nick,
                                       e.PrivateMessage.Message));
                        }
                    }
                }
                catch (Exception exc)
                {
                    rawMessageQueue.Enqueue(
                           string.Format(
                               "PRIVMSG {0} :Exception thrown: {1}: {2}",
                               ConfigHelper.Config.ReportingChannel,
                                exc.Message,
                               exc.Source));
                }
            }
        }

        private void HandleUserJoinedChannel(object sender, ChannelUserEventArgs e)
        {
            var client = sender as IrcClient;

            if (!this.ignoredChannels.Contains(e.Channel.Name.ToLower()))
            {
                // Use pLinq here to run the exploit finders in parallel
                // And write the logic to wait for the first intolerated exploit and
                // trigger the kick
                var exploitTypes = this.exploitFinders.Run(new UserInfo(e.User)).ToArray();


                var exploitsString = string.Empty;

                exploitsString = string.Join(",", exploitTypes.Select(t => t.Description));

                if (ConfigHelper.Config.BeVerbose || exploitTypes.Any())
                {
                    var untoleratedExploitTypes = this.untoleratedExploits[ConfigHelper.Config.ToleranceLevel];

                    if (exploitTypes.Select(t => t.Type).Any(untoleratedExploitTypes.Select(u => u.Type).Contains))
                    {

                        rawMessageQueue.Enqueue
                        (string.Format(
                                "PRIVMSG {0} :KICK Trigger: in {1}, Nick: {2}, Host {3}, Exploits: {4}",
                                ConfigHelper.Config.ReportingChannel,
                                e.Channel.Name,
                                e.User.Nick,
                                e.User.Hostname,
                                exploitTypes.Any() ? exploitsString : "None"));

                        if (e.Channel != null && e.Channel.UsersByMode.ContainsKey('o') && e.Channel.UsersByMode['o'].Any(x => x.Nick == client.User.Nick))
                        {
                            client.SendRawMessage("MODE {0} +b *!*@{1}", e.Channel.Name, e.User.Hostname);

                            e.Channel.Kick(e.User.Nick, ConfigHelper.Config.KickReason);
                        }
                    }
                    else
                    {
                        rawMessageQueue.Enqueue
                        (
                            string.Format
                            (
                                "PRIVMSG {0} :Joins {1}, Nick: {2}, Host {3}, Exploits: {4}",
                                ConfigHelper.Config.ReportingChannel,
                                e.Channel.Name,
                                e.User.Nick,
                                e.User.Hostname,
                                exploitTypes.Any() ? exploitsString : "None"));
                    }
                }
            }
        }

        private Dictionary<int, IEnumerable<Exploit>> GetUntoleratedExploitTypes()
        {
            return new Dictionary<int, IEnumerable<Exploit>>()
                                      {
                                          {10, new [] {new Exploit(ExploitType.Banlist_Match)}},
                                          {9, new [] {new Exploit(ExploitType.Banlist_Match)}},
                                          {8, new [] {new Exploit(ExploitType.Banlist_Match)}},
                                          {7, new [] {new Exploit(ExploitType.Banlist_Match), new Exploit(ExploitType.Nick_Abuser)}},
                                          {6, new [] {new Exploit(ExploitType.Banlist_Match), new Exploit(ExploitType.Nick_Abuser)}},
                                          {5, new [] {new Exploit(ExploitType.Banlist_Match), 
                                              new Exploit(ExploitType.Nick_Abuser)}},
                                          {4, new [] {new Exploit(ExploitType.Banlist_Match), 
                                              new Exploit(ExploitType.Nick_Abuser),
                                          new Exploit(ExploitType.OpenWingateProxy),
                                          new Exploit(ExploitType.SocksProxy),
                                          new Exploit(ExploitType.ProxyChain),
                                          }},
                                          {3, new [] {new Exploit(ExploitType.Banlist_Match), 
                                              new Exploit(ExploitType.Nick_Abuser),
                                          new Exploit(ExploitType.OpenProxy),
                                          new Exploit(ExploitType.OpenWingateProxy),
                                          new Exploit(ExploitType.HTTPProxy),
                                          new Exploit(ExploitType.SocksProxy),
                                          new Exploit(ExploitType.IRCDrone),
                                          new Exploit(ExploitType.Bottler),
                                          new Exploit(ExploitType.ProxyChain),
                                          new Exploit(ExploitType.AutomaticallyDeterminedBotnet),
                                          new Exploit(ExploitType.DDOSDrone),
                                          new Exploit(ExploitType.UnknownSpambotOrDrone),
                                          }},
                                          {2, new [] {new Exploit(ExploitType.Banlist_Match), 
                                              new Exploit(ExploitType.Nick_Abuser),
                                          new Exploit(ExploitType.OpenProxy),
                                          new Exploit(ExploitType.OpenWingateProxy),
                                          new Exploit(ExploitType.HTTPProxy),
                                          new Exploit(ExploitType.SocksProxy),
                                          new Exploit(ExploitType.IRCDrone),
                                          new Exploit(ExploitType.Bottler),
                                          new Exploit(ExploitType.ProxyChain),
                                          new Exploit(ExploitType.AutomaticallyDeterminedBotnet),
                                          new Exploit(ExploitType.DDOSDrone),
                                          new Exploit(ExploitType.UnknownSpambotOrDrone),
                                          }},{1, new [] {new Exploit(ExploitType.Banlist_Match), 
                                              new Exploit(ExploitType.Nick_Abuser),
                                          new Exploit(ExploitType.OpenProxy),
                                          new Exploit(ExploitType.OpenWingateProxy),
                                          new Exploit(ExploitType.HTTPProxy),
                                          new Exploit(ExploitType.SocksProxy),
                                          new Exploit(ExploitType.IRCDrone),
                                          new Exploit(ExploitType.Bottler),
                                          new Exploit(ExploitType.ProxyChain),
                                          new Exploit(ExploitType.AutomaticallyDeterminedBotnet),
                                          new Exploit(ExploitType.DDOSDrone),
                                          new Exploit(ExploitType.UnknownSpambotOrDrone),
                                          }},
                                      };
        }
    }
}