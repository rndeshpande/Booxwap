namespace Booxwap.Mvc.Models
{
    using Newtonsoft.Json;

    public class LocationModel
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