Update: December 17, 2017: We now have a Docker build. Try out the Dockerfile in the source or you can use our official image.

```
docker pull fahadash/ashscan
docker run -t --rm fahadash/ashscan /ashscan/dockerstart.sh
```

# ashscan
R&amp;D IRC Bot project. Was initially created to slow down the flooders and abusers. But now a fully extensible addin-enabled project that can allow addition of external features through separate addin DLLs. There are a few addins written already and can be found in in [Addins directory](./Projects/Addins). Check the respective Readme.md file of each Addin to find more about them. Writing your own addins is a piece of cake, [click here](./docs/Writing-Addin.md) to find out everything you need to know about Creating and Deploying Addins.

It is a basic bot with configuration parameters in app.config. We have decoupled the Exploit Finder and Protection-Bot feature out to a separate plugin. You can choose whether or not to include that in your bot. It checks against DNS-Blacklistss, Banned Hostlists maintained in a text file, word abuse in nicknames. 

So now ashscan is just a plugin enabled bot with no features without plugins.

ashscan is using Chatsharp as IRC Library, but it is loosely coupled to that so you can rip it out and switch it to another IRC library of your choice if you have to.

Thanks to XML Manifest support of Mono.Addins, now you can easily add new DNS-Blacklist Service without having to write any C# code. [Click here](./docs/DNS-BL.md) to find out how.

Launching your protection bot is very easy. Steps are

1. Download the source, make a build in Visual Studio or Xamarin (ashscan.Bot is the project you want)
2. Edit the app.config file to change bot nicks, nickserv passwords, etc
3. Double click the exe file or launch it using Mono (if using linux)

[Click here](./docs/run-on-linux.md) to find out how you can build and run it under Linux.


The app now is in stable state. Thanks to our new contributor KhawajaUmerFarooq (aka Diabolic)

### For Developers
Feel free to fork this project, bring in your good ideas and send us a pull request. [Click here](./docs/Setting-up-build-environment.md) to find out how to get you started.

For more information on how to use this project including adding new DNS-Blacklist services or writing addins. Refer to the documents in docs folder.

Special thanks to Drew Dewalt aka SirCmpwn for his ChatSharp repository 


