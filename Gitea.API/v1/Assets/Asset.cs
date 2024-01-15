using System;
using Gitea.API.v1.Releases;
using Newtonsoft.Json;

namespace Gitea.API.v1.Assets
{
    public class Asset: JsonEntityBase
    {
        [JsonProperty("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("download_count")]
        public int DownloadCount { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }
        
        public Release Release { get; internal set; }
    }
}