# ashscan
R&amp;D Project to stop bad guys on IRC Chat rooms, A good and basic exercise to fight hackers.

It is a basic bot with configuration parameters in app.config. It checks against DNS-BLs, Banned Hostlists, word abuse in nicknames. Working on this project can help you exercise the realtime data processing a bit as well as how attackers and abusers use exploit to their advantage and what can we do to stop them.

Launching your protection bot is very easy. Steps are
1. Download the source, make a build in Visual Studio or Xamarin (IRCExploitChecker is the project you want)
2. Edit the app.config file to change bot nicks, nickserv passwords, etc
3. Double click the exe file or launch it using Mono (if using linux)

The app is currently not fully stable. We would appreciate your help.

Special thanks to SirCmpwn for his ChatSharp repository which really made this possible.
