namespace Booxwap.Social.FacebookHelpers
{
    using Newtonsoft.Json;

    internal class FbsqlModel
    {
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("uid")]
        public string Id
        {
            get;
            set;
        }
    }
}