ashscan can run under Linux using Mono. The projects in this repository are targetted to .NET Framework 4.5 and require Mono 3.x.x.

The following steps can help you install and run the but under Linux.


1. Install Mono by following the instructions on their page here. http://www.mono-project.com/docs/getting-started/install/linux/
2. Edit the app.config file and enable the appSettings and edit the two file paths for banlist and banwords. Make sure the file paths are fully qualified and they are Linux-style paths
3. On the directory where you have ashscan repo downloaded, follow the instructions on this blog post to download NuGet.exe into .nuget directory. (You can disregard the portion about compiling and configuring for windows) http://dlafferty.blogspot.com/2013/08/building-your-microsoft-solution-with.html
4. Run xbuild ExploitChecker.sln


Once the solution is built, change the directory to ./IrcExploitChecker.App/Debug/bin and run IrcExploitChecker.App.exe


To compile addins, 

1. Go to ashscan/addins and compile the addins csproj files using xbuild command line and copy their output to ashscan/IrcExploitChecker.App/Debug/bin/addins (create the addins folder if it doesn't exist)
2. Copy the file ExploitChecker.addins from Extensibility project directory to ashscan/IrcExploitChecker.App/Debug/bin
3. Launch the IrcExploitChecker.App.exe


We will soon be creating a new build target for Mono to make the above process easier
