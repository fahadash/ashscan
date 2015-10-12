# DNS Blacklist Service
There are plenty of DNS Blacklist Services such as DroneBL (www.dronebl.org).

To add a new service, read their documentation on what suffix they add to the reversed IP for instance dronebl adds dnsbl.dronebl.org.

For more information on DroneBL usage see http://dronebl.org/docs/howtouse


Load the project and edit the ashscan/Projects/ExploitChecker/ExploitFinder.addin.xml

Under the node of ``` <Extension path ="/ExploitFinder/HostExploitFinder">``` add a new node for your new DNS BL Service like the following

```xml
    <DnsBlacklistProvider  ProviderName="DroneBl" HostSuffix="dnsbl.dronebl.org" >
      <ExploitCode  Code="2" Description="IRC Drone"  />
      <ExploitCode  Code="5" Description="Bottler"  />
      <ExploitCode  Code="6" Description="Unknown Spambot or Drone"  />
      <ExploitCode  Code="7" Description="DDOS Drone"  />
      <ExploitCode Code="8" Description="Socks Proxy"  />
      <ExploitCode Code="9" Description="HTTP Proxy"  />
      <ExploitCode  Code="10" Description="Proxy Chain"  />
      <ExploitCode  Code="13" Description="Brute Force Attackers"  />
      <ExploitCode  Code="14" Description="Open Wingate Proxy"  />
      <ExploitCode Code="15" Description="Compromised router / gateway"  />
      <ExploitCode  Code="17" Description="Automatically determined botnet"  />
    </DnsBlacklistProvider>
```

The code to description mapping can be obtained from the DNS-BL Service's website.

You will need to recompile the ExploitFinder plugin and and deploy the binaries into the addins/exploitfinder. It will be picked up when you restart your bot.

The alternate way to compiling is using External Mono.Addin manifests. See more here: https://github.com/mono/mono-addins/wiki/Add-in-Discovery
