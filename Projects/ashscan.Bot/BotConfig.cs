using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashscan.Bot
{
    public class BotConfig : ConfigurationSection
    {
        [ConfigurationProperty("Nick", IsRequired = true)]
        public string Nick
        {
            get { return (string)this["Nick"]; }
            set { this["Nick"] = value; }
        }

        [ConfigurationProperty("Username", IsRequired = true)]
        public string Username
        {
            get { return (string)this["Username"]; }
            set { this["Username"] = value; }
        }

        [ConfigurationProperty("Fullname", IsRequired = false, DefaultValue="Unknown")]
        public string Fullname
        {
            get { return (string)this["Fullname"]; }
            set { this["Fullname"] = value; }
        }

        [ConfigurationProperty("BlackListedHostsPath", IsRequired = true)]
        public string BlackListedHostsPath
        {
            get { return (string)this["BlackListedHostsPath"]; }
            set { this["BlackListedHostsPath"] = value; }
        }

        [ConfigurationProperty("AbusiveWordsPath", IsRequired = true)]
        public string AbusiveWordsPath
        {
            get { return (string)this["AbusiveWordsPath"]; }
            set { this["AbusiveWordsPath"] = value; }
        }

        [ConfigurationProperty("NickservPassword", IsRequired = false, DefaultValue="")]
        public string NickservPassword
        {
            get { return (string)this["NickservPassword"]; }
            set { this["NickservPassword"] = value; }
        }


        [ConfigurationProperty("ToleranceLevel", IsRequired = true, DefaultValue=5)]
        public int ToleranceLevel
        {
            get { return (int)this["ToleranceLevel"]; }
            set { this["ToleranceLevel"] = value; }
        }

        [ConfigurationProperty("Network", IsRequired = true)]
        public string Network
        {
            get { return (string)this["Network"]; }
            set { this["Network"] = value; }
        }

        [ConfigurationProperty("WatchedChannels", IsRequired = true)]
        public string WatchedChannels
        {
            get { return (string)this["WatchedChannels"]; }
            set { this["WatchedChannels"] = value; }
        }

        [ConfigurationProperty("ReportingChannel", IsRequired = true, DefaultValue="#ash")]
        public string ReportingChannel
        {
            get { return (string)this["ReportingChannel"]; }
            set { this["ReportingChannel"] = value; }
        }

        [ConfigurationProperty("BotOperators", IsRequired = true)]
        public string BotOperators
        {
            get { return (string)this["BotOperators"]; }
            set { this["BotOperators"] = value; }
        }


        [ConfigurationProperty("KickReason", IsRequired = false, DefaultValue = "I don't know")]
        public string KickReason
        {
            get { return (string)this["KickReason"]; }
            set { this["KickReason"] = value; }
        }


        [ConfigurationProperty("LongNickLength", DefaultValue=20)]
        public int LongNickLength
        {
            get { return (int)this["LongNickLength"]; }
            set { this["LongNickLength"] = value; }
        }

        [ConfigurationProperty("Verbose", IsRequired = false, DefaultValue=false)]
        public bool BeVerbose
        {
            get { return (bool)this["Verbose"]; }
            set { this["Verbose"] = value; }
        }


        [ConfigurationProperty("AutoReconnect", IsRequired = false, DefaultValue = false)]
        public bool AutoReconnect
        {
            get { return (bool)this["AutoReconnect"]; }
            set { this["AutoReconnect"] = value; }
        }


        [ConfigurationProperty("AutoReconnectTime", IsRequired = false, DefaultValue = 10)]
        public int AutoReconnectTimer
        {
            get { return (int)this["AutoReconnectTime"]; }
            set { this["AutoReconnectTime"] = value; }
        }
    }

    public class ConfigHelper
    {
        public static BotConfig Config {get;set;}

        static ConfigHelper()
        {
            Config = (BotConfig)ConfigurationManager.GetSection("BotConfig/Config");
        }
    }
}
