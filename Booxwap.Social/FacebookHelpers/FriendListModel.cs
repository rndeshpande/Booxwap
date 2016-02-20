namespace Booxwap.Social.FacebookHelpers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class FriendListModel
    {
        [JsonProperty("data")]
        public IList<FriendModel> List
        {
            get;
            set;
        }
    }
}