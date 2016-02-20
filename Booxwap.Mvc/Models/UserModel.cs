namespace Booxwap.Mvc.Models
{
    using Newtonsoft.Json;

    public class UserModel
    {
        [JsonProperty("id")]
        public string Id
        {
            get;
            set;
        }

        [JsonProperty("first_name")]
        public string FirstName
        {
            get;
            set;
        }

        [JsonProperty("gender")]
        public string Gender
        {
            get;
            set;
        }

        [JsonProperty("last_name")]
        public string LastName
        {
            get;
            set;
        }

        [JsonProperty("link")]
        public string FbLink
        {
            get;
            set;
        }

        [JsonProperty("location")]
        public LocationModel Location
        {
            get;
            set;
        }

        [JsonProperty("name")]
        public string FullName
        {
            get;
            set;
        }

        [JsonProperty("username")]
        public string Username
        {
            get;
            set;
        }
    }
}