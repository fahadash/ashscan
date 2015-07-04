# ashscan
R&amp;D IRC Bot project. Was initially created to slow down the flooders and abusers. But now a fully plugin-enabled project that can allow addition of external features through separate addin DLLs. 

It is a basic bot with configuration parameters in app.config. It checks against DNS-BLs, Banned Hostlists, word abuse in nicknames. Working on this project can help you get a grip on some realtime data processing a bit as well as how attackers and abusers use exploit to their advantage and what can we do to stop them.

Thanks to XML Manifest support of Mono.Addins, now you can easily add new DNS-BL Service without having to write any C# code.

Launching your protection bot is very easy. Steps are

1. Download the source, make a build in Visual Studio or Xamarin (IRCExploitChecker.App is the project you want)
2. Edit the app.config file to change bot nicks, nickserv passwords, etc
3. Double click the exe file or launch it using Mono (if using linux)

Here is how you can run it under Linux
https://github.com/fahadash/ashscan/blob/master/docs/run-on-linux.md

The app now is in stable state. Thanks to our new contributor UmerFarooqKhawaja (aka Diabolic)

For more information on how to use this project including adding new DNS-BL services or writing plugins. Refer to the documents in docs folder.

Special thanks to SirCmpwn for his ChatSharp repository 


