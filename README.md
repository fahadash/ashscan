# ashscan
R&amp;D IRC Bot project. Was initially created to slow down the flooders and abusers. But now a fully plugin-enabled project that can allow addition of external features through separate addin DLLs. 

It is a basic bot with configuration parameters in app.config. It checks against DNS-BLs, Banned Hostlists, word abuse in nicknames. 

Thanks to XML Manifest support of Mono.Addins, now you can easily add new DNS-BL Service without having to write any C# code. Here is how: https://github.com/fahadash/ashscan/blob/master/docs/DNS-BL.md

Launching your protection bot is very easy. Steps are

1. Download the source, make a build in Visual Studio or Xamarin (ashscan.Bot is the project you want)
2. Edit the app.config file to change bot nicks, nickserv passwords, etc
3. Double click the exe file or launch it using Mono (if using linux)

Here is how you can build and run it under Linux
https://github.com/fahadash/ashscan/blob/master/docs/run-on-linux.md

The app now is in stable state. Thanks to our new contributor KhawajaUmerFarooq (aka Diabolic)

For more information on how to use this project including adding new DNS-BL services or writing plugins. Refer to the documents in docs folder.

Special thanks to SirCmpwn for his ChatSharp repository 


