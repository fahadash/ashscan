#Writing Addins

Every once in a while you would need some functionality in the bot that does not existed. You don't want to have to learn the details of how IRC protocol is handled by the app. You just have a few commands to add to the bot. Addins is the answer.

There are two addin projects inside Projects/Addins. Deploying addins is simple, you just copy the DLL of an addin along with dependencies into the appdir/addins directory and reload your app.


Here is how a typical addin is written. We are using Mono.Addins for modularity so the first step is to add the Addin attribute to AssemblyInfo.cs of your class library project.

```CSharp
[assembly: Addin]
[assembly: AddinDependency("Extensibility", "1.0")]
```

Then add the reference to Extensibility.dll which is one of the projects.

Currently an addin can add command handlers (active chat listening is part of future enhancements)

So our new command handler class will be derived from ICommandHandler

```CSharp
    [Extension]
    public class OwnerCommandsHandler : ICommandHandler
    {

        // Controller is needed and fetched through Mono.Addins engine if you need to interact with any of the
        // IRC Client feature
        private IIrcController controller = null;

        public OwnerCommandsHandler()
        {
           // You need to request the controller if you want your addin to interact with IRC client.
            this.controller = ExtensionManager.GetController();
        }
        
        // This tells the Engine what commands are handled by this handler. These commands will be delivered to you.
        // System picks the first handler found in the list of handlers for a particular command
        public IEnumerable<string> Commands
        {
            get
            {
                return new[] { "join", "part" };
            }
        }

        // This tells the system whether commands for this command handler can be called by a bot operator only
        public bool OperatorsOnly
        {
            get
            {
                return true;
            }
        }
        // Tokens of the command are passed as array created by splitting the string using space as the delimiter
        public void Handle(IUserInfo oper, IEnumerable<string> tokens)
        {
            var split = tokens.ToArray();

            if (split.Length >= 2)
            {
                var command = split[0];

                var channel = split[1];

                    switch (command.ToLower())
                    {
                    // Join channel command
                        case "join":
                            if (split.Length == 2 && split[1].StartsWith("#"))
                            {
                                controller.Join(channel);
                            }
                            break;
                    // Say something in a channel
                        case "channelmessage":
                            if (split.Length => 3 && split[1].StartsWith("#"))
                            {
                                controller.Say(channel, string.Join(' ', split.Skip(2));
                            }
                            break;
                    }
            }
        }
    }
```
