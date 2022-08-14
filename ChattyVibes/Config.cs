using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.IO;

namespace ChattyVibes
{
    class Config
    {
        #region Twitch
        [JsonProperty("Twitch.Username", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue("")]
        public string TwitchUsername = "";
        #endregion Twitch

        #region Buttplug
        [JsonProperty("Buttplug.Hostname", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue("localhost")]
        public string ButtplugHostname = "localhost";
        [JsonProperty("Buttplug.Port", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(12345)]
        public uint ButtplugPort = 12345;
        #endregion Buttplug

        #region Binding - Message
        [JsonProperty("Binding.Message.Vibe.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindMsgVibeEna = false;
        [JsonProperty("Binding.Message.Vibe.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindMsgVibeLvl = .02f;
        [JsonProperty("Binding.Message.Rotate.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindMsgRotEna = false;
        [JsonProperty("Binding.Message.Rotate.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindMsgRotLvl = .02f;
        [JsonProperty("Binding.Message.Rotate.Clockwise", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(true)]
        public bool BindMsgRotClock = true;
        [JsonProperty("Binding.Message.Stroke.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindMsgStrEna = false;
        [JsonProperty("Binding.Message.Stroke.Minimum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindMsgStrMin = .02f;
        [JsonProperty("Binding.Message.Stroke.Maximum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.08f)]
        public float BindMsgStrMax = .08f;
        [JsonProperty("Binding.Message.Duration", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(1500)]
        public int BindMsgDuration = 1500;
        #endregion Binding - Message

        #region Binding - Whisper
        [JsonProperty("Binding.Whisper.Vibe.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindWhisperVibeEna = false;
        [JsonProperty("Binding.Whisper.Vibe.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindWhisperVibeLvl = .02f;
        [JsonProperty("Binding.Whisper.Rotate.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindWhisperRotEna = false;
        [JsonProperty("Binding.Whisper.Rotate.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindWhisperRotLvl = .02f;
        [JsonProperty("Binding.Whisper.Rotate.Clockwise", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(true)]
        public bool BindWhisperRotClock = true;
        [JsonProperty("Binding.Whisper.Stroke.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindWhisperStrEna = false;
        [JsonProperty("Binding.Whisper.Stroke.Minimum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindWhisperStrMin = .02f;
        [JsonProperty("Binding.Whisper.Stroke.Maximum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.08f)]
        public float BindWhisperStrMax = .08f;
        [JsonProperty("Binding.Whisper.Duration", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(1500)]
        public int BindWhisperDuration = 1500;
        #endregion Binding - Whisper

        #region Binding - New Sub
        [JsonProperty("Binding.NewSub.Vibe.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindNewSubVibeEna = false;
        [JsonProperty("Binding.NewSub.Vibe.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindNewSubVibeLvl = .02f;
        [JsonProperty("Binding.NewSub.Rotate.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindNewSubRotEna = false;
        [JsonProperty("Binding.NewSub.Rotate.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindNewSubRotLvl = .02f;
        [JsonProperty("Binding.NewSub.Rotate.Clockwise", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(true)]
        public bool BindNewSubRotClock = true;
        [JsonProperty("Binding.NewSub.Stroke.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindNewSubStrEna = false;
        [JsonProperty("Binding.NewSub.Stroke.Minimum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindNewSubStrMin = .02f;
        [JsonProperty("Binding.NewSub.Stroke.Maximum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.08f)]
        public float BindNewSubStrMax = .08f;
        [JsonProperty("Binding.NewSub.Duration", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(1500)]
        public int BindNewSubDuration = 1500;
        #endregion Binding - New Sub

        #region Binding - Gifted Sub
        [JsonProperty("Binding.GiftedSub.Vibe.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindGiftSubVibeEna = false;
        [JsonProperty("Binding.GiftedSub.Vibe.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindGiftSubVibeLvl = .02f;
        [JsonProperty("Binding.GiftedSub.Rotate.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindGiftSubRotEna = false;
        [JsonProperty("Binding.GiftedSub.Rotate.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindGiftSubRotLvl = .02f;
        [JsonProperty("Binding.GiftedSub.Rotate.Clockwise", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(true)]
        public bool BindGiftSubRotClock = true;
        [JsonProperty("Binding.GiftedSub.Stroke.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindGiftSubStrEna = false;
        [JsonProperty("Binding.GiftedSub.Stroke.Minimum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindGiftSubStrMin = .02f;
        [JsonProperty("Binding.GiftedSub.Stroke.Maximum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.08f)]
        public float BindGiftSubStrMax = .08f;
        [JsonProperty("Binding.GiftedSub.Duration", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(1500)]
        public int BindGiftSubDuration = 1500;
        #endregion Binding - Gifted Sub

        #region Binding - Continued Gifted Sub
        [JsonProperty("Binding.ContinuedGiftedSub.Vibe.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindContGiftSubVibeEna = false;
        [JsonProperty("Binding.ContinuedGiftedSub.Vibe.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindContGiftSubVibeLvl = .02f;
        [JsonProperty("Binding.ContinuedGiftedSub.Rotate.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindContGiftSubRotEna = false;
        [JsonProperty("Binding.ContinuedGiftedSub.Rotate.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindContGiftSubRotLvl = .02f;
        [JsonProperty("Binding.ContinuedGiftedSub.Rotate.Clockwise", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(true)]
        public bool BindContGiftSubRotClock = true;
        [JsonProperty("Binding.ContinuedGiftedSub.Stroke.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindContGiftSubStrEna = false;
        [JsonProperty("Binding.ContinuedGiftedSub.Stroke.Minimum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindContGiftSubStrMin = .02f;
        [JsonProperty("Binding.ContinuedGiftedSub.Stroke.Maximum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.08f)]
        public float BindContGiftSubStrMax = .08f;
        [JsonProperty("Binding.ContinuedGiftedSub.Duration", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(1500)]
        public int BindContGiftSubDuration = 1500;
        #endregion Binding - Continued Gifted Sub

        #region Binding - Community Sub
        [JsonProperty("Binding.CommunitySub.Vibe.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindComSubVibeEna = false;
        [JsonProperty("Binding.CommunitySub.Vibe.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindComSubVibeLvl = .02f;
        [JsonProperty("Binding.CommunitySub.Rotate.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindComSubRotEna = false;
        [JsonProperty("Binding.CommunitySub.Rotate.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindComSubRotLvl = .02f;
        [JsonProperty("Binding.CommunitySub.Rotate.Clockwise", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(true)]
        public bool BindComSubRotClock = true;
        [JsonProperty("Binding.CommunitySub.Stroke.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindComSubStrEna = false;
        [JsonProperty("Binding.CommunitySub.Stroke.Minimum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindComSubStrMin = .02f;
        [JsonProperty("Binding.CommunitySub.Stroke.Maximum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.08f)]
        public float BindComSubStrMax = .08f;
        [JsonProperty("Binding.CommunitySub.Duration", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(1500)]
        public int BindComSubDuration = 1500;
        #endregion Binding - Community Sub

        #region Binding - Prime Sub
        [JsonProperty("Binding.PrimeSub.Vibe.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindPrimeSubVibeEna = false;
        [JsonProperty("Binding.PrimeSub.Vibe.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindPrimeSubVibeLvl = .02f;
        [JsonProperty("Binding.PrimeSub.Rotate.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindPrimeSubRotEna = false;
        [JsonProperty("Binding.PrimeSub.Rotate.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindPrimeSubRotLvl = .02f;
        [JsonProperty("Binding.PrimeSub.Rotate.Clockwise", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(true)]
        public bool BindPrimeSubRotClock = true;
        [JsonProperty("Binding.PrimeSub.Stroke.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindPrimeSubStrEna = false;
        [JsonProperty("Binding.PrimeSub.Stroke.Minimum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindPrimeSubStrMin = .02f;
        [JsonProperty("Binding.PrimeSub.Stroke.Maximum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.08f)]
        public float BindPrimeSubStrMax = .08f;
        [JsonProperty("Binding.PrimeSub.Duration", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(1500)]
        public int BindPrimeSubDuration = 1500;
        #endregion Binding - Prime Sub

        #region Binding - Re-Sub
        [JsonProperty("Binding.ReSub.Vibe.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindReSubVibeEna = false;
        [JsonProperty("Binding.ReSub.Vibe.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindReSubVibeLvl = .02f;
        [JsonProperty("Binding.ReSub.Rotate.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindReSubRotEna = false;
        [JsonProperty("Binding.ReSub.Rotate.Level", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindReSubRotLvl = .02f;
        [JsonProperty("Binding.ReSub.Rotate.Clockwise", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(true)]
        public bool BindReSubRotClock = true;
        [JsonProperty("Binding.ReSub.Stroke.Enable", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(false)]
        public bool BindReSubStrEna = false;
        [JsonProperty("Binding.ReSub.Stroke.Minimum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.02f)]
        public float BindReSubStrMin = .02f;
        [JsonProperty("Binding.ReSub.Stroke.Maximum", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(.08f)]
        public float BindReSubStrMax = .08f;
        [JsonProperty("Binding.ReSub.Duration", DefaultValueHandling = DefaultValueHandling.Populate), DefaultValue(1500)]
        public int BindReSubDuration = 1500;
        #endregion Binding - Re-Sub

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
