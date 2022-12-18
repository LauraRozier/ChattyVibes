using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.IO;

namespace ChattyVibes
{
    class Config
    {
        #region Twitch
        [JsonProperty("Twitch.Username", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue("ChattyVibes")]
        public volatile string TwitchUsername = "ChattyVibes";
        [JsonProperty("Twitch.ChannelName", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue("ChattyVibes")]
        public volatile string ChannelName = "ChattyVibes";
        [JsonProperty("Twitch.ClientId", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue("77nu3r5gyqhuzsambceccrbd9ctjdo")]
        public volatile string TwitchClientId = "77nu3r5gyqhuzsambceccrbd9ctjdo";
        #endregion Twitch

        #region Buttplug
        [JsonProperty("Buttplug.Hostname", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue("localhost")]
        public volatile string ButtplugHostname = "localhost";
        [JsonProperty("Buttplug.Port", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(12345u)]
        public volatile uint ButtplugPort = 12345u;
        #endregion Buttplug


        public static void Load(ref Config aConf)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            if (File.Exists(@"./config.json"))
            {
                using (var sr = new StreamReader(@"./config.json"))
                    using (var reader = new JsonTextReader(sr))
                        aConf = serializer.Deserialize<Config>(reader);
            }
            else
            {
                aConf = new Config();
            }
        }

        public static void Save(Config aConf)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var sw = new StreamWriter(@"./config.json"))
                using (var writer = new JsonTextWriter(sw))
                    serializer.Serialize(writer, aConf);
        }
    }
}
