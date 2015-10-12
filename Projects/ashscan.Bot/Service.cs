using System;
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
        
        public Service()
            : base()
        {

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

            ircClient = new IrcClient( ConfigHelper.Config.Network, new IrcUser(ConfigHelper.Config.Nick, ConfigHelper.Config.Username, string.Empty, ConfigHelper.Config.Fullname));

            ExtensionManager.Initialize();

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

                Thread.Sleep(TimeSpan.FromSeconds(seconds));
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
          
        }        
    }
}