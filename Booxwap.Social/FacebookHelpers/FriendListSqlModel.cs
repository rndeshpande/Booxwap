namespace Booxwap.Social.FacebookHelpers
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class FriendListSqlModel
    {
        [JsonProperty("data")]
        public IList<FbsqlModel> List
        {
            get;
            set;
        }
    }
}