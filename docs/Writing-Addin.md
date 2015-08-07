#Writing Addins

Every once in a while you would need some functionality in the bot that does not exist. You don't want to have to learn the details of how IRC protocol is handled by the app. You just have a few commands to add to the bot. Addins is the answer.

There are two addin projects inside Projects/Addins. Deploying addins is simple, you just copy the DLL of an addin along with dependencies into the appdir/addins directory and reload your app.

### Lets Go
Here is how a typical addin is written. We will write a simple Hello World Addin

#### Start a new Addin Class Library Project
Each Addin is a DLL created using C# Library Project. In Visual Studio start a new project, in the project type choose 'Class Library' (we prefer you use C# as your language, but you could use any .NET language)

#### Add reference to Mono.Addins using Nuget package manager
Once you have the new project created, now the time is to add reference to Mono.Addins which you can do using Nuget. Right-Click on References under your project and choose 'Manage Nuget Packages'. If you don't see that menu item, you may have to install/update Nuget Package Manager in your Visual Studio.

Once you have the package dialog open, go to Online from the left-hand navigation pane and on the search bar in top right corner, type in Mono.Addins. Choose Mono.Addins from search results and click Install.

#### Add reference to Extensibility.dll
Now that you have Mono.Addins, it is time to add reference to Extensibility.dll. Extensibility is a class library project required to be referenced by every addin. It is part of ashscan solution. If you are working directly in ashscan solution, you can simply right click on References under your addin project and click Add Reference and in the dialog box, choose Solutions on the left-hand navigation pane, and within the list of Projects, check "Extensibility". If you are working on your own solution, then locate the Extensibility.dll wherever you may have it compiled and add it using "Browse" option.

#### Edit Assembly.info of your addin
We are using Mono.Addins for modularity so the first step is to add the Addin attribute to AssemblyInfo.cs of your class library project. AssemblyInfo.cs file should be located under Adding-Project/Properties.

```CSharp
using Mono.Addins; // This should be placed on top among all other usings
[assembly: Addin]
[assembly: AddinDependency("Extensibility", "1.0")]
```


This example is a Command Handler addin which listens to commands in a private window so our new command handler class will be derived from ICommandHandler

Rename Class1 to HelloWorldHandler and change the content of the class to the following.

```CSharp
    using Extensibility;
    using Mono.Addins;

    [Extension]
    public class HelloWorldHandler : ICommandHandler
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
                return new[] { "hello" };
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
            var command = tokens.First();
            switch (command.ToLower())
            {
            // hello command
                case "hello":
                    controller.Say(user.Nick, "hello");
                    break;
            }
        }
    }
```

#### Building and Deploying
Once you are done building an addin, its time to deploy. A typical deployment is easy, you copy all the DLLs generated inside Your-Plugin/Bin/Debug folder to ashscan.Bot/Bin/Debug/addins/New-Addin folder, but there are ways you can customize it.

