using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatSharp;
using ChatSharp.Events;

namespace TestChatSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new IrcClient("irc.freenode.net", new IrcUser("ChatSharp", "ChatSharp"));
            client.NetworkError += (s, e) => Console.WriteLine("Error: " + e.SocketError);
            client.RawMessageRecieved += (s, e) => Console.WriteLine("<< {0}", e.Message);
            client.RawMessageSent += (s, e) => Console.WriteLine(">> {0}", e.Message);
            client.UserMessageRecieved += (s, e) =>
                {
                    if (e.PrivateMessage.Message.StartsWith(".join "))
                        client.Channels.Join(e.PrivateMessage.Message.Substring(6));
                    else if (e.PrivateMessage.Message.StartsWith(".list "))
                    {
                        var channel = client.Channels[e.PrivateMessage.Message.Substring(6)];
                        var list = channel.Users.Select(u => u.Nick).Aggregate((a, b) => a + "," + b);
                        client.SendMessage(list, e.PrivateMessage.User.Nick);
                    }
                    else if (e.PrivateMessage.Message.StartsWith(".whois "))
                        client.WhoIs(e.PrivateMessage.Message.Substring(7), null);
                    else if (e.PrivateMessage.Message.StartsWith(".raw "))
                        client.SendRawMessage(e.PrivateMessage.Message.Substring(5));
                    else if (e.PrivateMessage.Message.StartsWith(".mode "))
                    {
                        var parts = e.PrivateMessage.Message.Split(' ');
                        client.ChangeMode(parts[1], parts[2]);
                    }
                };
            client.ChannelMessageRecieved += (s, e) =>
                {
                    Console.WriteLine("<{0}> {1}", e.PrivateMessage.User.Nick, e.PrivateMessage.Message);
                };
            client.ConnectAsync();
            while (true) ;
        }
    }
}
