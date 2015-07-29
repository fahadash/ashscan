# Contributing to ashscan
Congratulations on thinking about contributing to ashscan. This page hopes to guide you through the process of getting you started.


## Things you are going to need
The only two things you need to get yourself started are

1. Microsoft Visual Studio 2013
2. Git SCM

You can download Git SCM from www.git-scm.com

It is recommended to read the Git Basics http://git-scm.com/book/en/v2/Getting-Started-Git-Basics

The first step is to get the solution, get it to compile, and launch a bot from your local computer.

### Get the code
Getting the code from the GitHub repository is easy. Once you have downloaded Git, you can issue commands right from the shell or Windows Users can find a utility called 'Git Bash' from their local computer and type in all the commands in there.

For the first timers, create a directory in your C drive (for windows users) to hold all the source code.

$ mkdir /c/development

The above command will create a directory called Development in your C:\ drive

once you have the directory created, change your current directory to your newly created directory

$ cd /c/development/

Once you are in the development directory, the following command can pull all the code from GitHub repository and copy it to your local drive. At this time, you do not need to have a GitHub account, but you do need it once you are ready to push your changes to the server.

$ git clone https://github.com/fahadash/ashscan.git

Once the above command finishes, you will have a directory called ashscan under your C:\Development directory. 

### Compiling the Code
Once you have code, launch Visual Studio 2013, go to File - Open, and open the solution file ashscan.sln located under C:\Development\ashscan

Once the solution file is open, you will see several projects under the Solution Explorer window. Find the project named ashscan.Bot and expand it. Find a file named 'app.config', open it and change the configuration parameters such as Nick, Server, Channels and compile it.

To compile the project, First right-click on ashscan.Bot and choose 'Set as startup project' then go to Build menu, and choose Build Solution.

### Run the code
Once you have the solution built, you can hit the 'Start' button on the toolbar to start the project under debugger. After launching, you will see a Console window showing all the output, and your bot should eventually connect and join all the configured channels.
