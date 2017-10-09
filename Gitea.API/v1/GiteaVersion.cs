using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Gitea.API.v1
{
    /// <summary>
    /// Stores a Gitea version.
    /// </summary>
    [DataContract]
    public class GiteaVersion
    {
        /// <summary>
        /// Gets the underlying client.
        /// </summary>
        public Client Client
        {
            get;
            internal set;
        }

        /// <summary>
        /// version
        /// </summary>
        [DataMember]
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}