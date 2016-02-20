namespace Booxwap.Social.FacebookHelpers
{
    using Newtonsoft.Json;

    internal class LocationModel
    {
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }
    }
}