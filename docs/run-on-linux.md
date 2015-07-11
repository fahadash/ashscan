ashscan can run under Linux using Mono. The projects in this repository are targetted to .NET Framework 4.5 and require Mono 3.x.x.

## Installation
The following steps can help you install and run the bot under Linux. They have been tested on 13.4 LTS, and OpenSUSE 13.2.


1. Install Mono by following the instructions on [their page here](http://www.mono-project.com/docs/getting-started/install/linux/) 
2. Edit the app.config file and enable the appSettings and edit the two file paths for banlist and banwords. Make sure the file paths are fully qualified and they are Linux-style paths
3. On the directory where you have ashscan repo downloaded, follow the instructions on [this blog post](http://dlafferty.blogspot.com/2013/08/building-your-microsoft-solution-with.html) to download NuGet.exe into .nuget directory. (You can disregard the portion about compiling and configuring for windows) Typically the following instructions work
  1. Change directory to clone path
  2. Run command `wget http://nuget.org/nuget.exe`
  3. Run this command to move the downloade file to the hidden .nuget directory
        `cp nuget.exe ./.nuget/NuGet.exe
  4. Change the file mode to allow execution
        `chmod a+x ./.nuget/NuGet.exe`
4. Run `xbuild ExploitChecker.sln`


Once the solution is built, change the directory to ./IrcExploitChecker.App/Debug/bin and run IrcExploitChecker.App.exe

To compile addins, 

1. Go to ashscan/addins and compile the addins csproj files using xbuild command line and copy their output to ashscan/IrcExploitChecker.App/Debug/bin/addins (create the addins directory if it doesn't exist)
2. Copy the file ExploitChecker.addins from Extensibility project directory to ashscan/IrcExploitChecker.App/Debug/bin
3. Launch the IrcExploitChecker.App.exe


## Troubleshooting

##### Compile error 'SendFailure' errors when nuget tries to download referenced packages.
You will need to download the right certificates to your local cert store. Typically the following four commands do the job
> $ sudo mozroots --import --machine --sync
> $ sudo certmgr -ssl -m https://go.microsoft.com
> $ sudo certmgr -ssl -m https://nugetgallery.blob.core.windows.net
> $ sudo certmgr -ssl -m https://nuget.org

For more info: https://monomvc.wordpress.com/2012/03/06/nuget-on-mono/

We will soon be creating a new build target for Mono to make the above process easier

##### ashscan terminates with File Not Found exceptions when starts
This is because the paths to two files `Banlist` and `Banned words list` are not configured properly and/or files don't exist at the specified path. You can use 'touch /path/to/filename' to create a blank file if it doesn't exist.
