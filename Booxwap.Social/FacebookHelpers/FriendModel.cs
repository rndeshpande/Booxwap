namespace Booxwap.Social.FacebookHelpers
{
    using Newtonsoft.Json;
    internal class FriendModel
    {
        [JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }
    }
}