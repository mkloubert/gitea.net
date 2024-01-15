using System;
using System.Collections.Generic;
using Gitea.API.v1.Assets;
using Gitea.API.v1.Repositories;
using Gitea.API.v1.Users;
using Newtonsoft.Json;

namespace Gitea.API.v1.Releases
{
    public class Release: JsonEntityBase
    {
        public Release()
        {
            AssetsEndpoint = new AssetsEndpoint(this);
        }

        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }

        [JsonProperty("author")]
        public User Author { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("draft")]
        public bool Draft { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("prerelease")]
        public bool Prerelease { get; set; }

        [JsonProperty("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("tarball_url")]
        public string TarballUrl { get; set; }

        [JsonProperty("target_commitish")]
        public string TargetCommitish { get; set; }

        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("zipball_url")]
        public string ZipBallUrl { get; set; }
        
        public Repository Repository { get; internal set; }
        
        public AssetsEndpoint AssetsEndpoint { get; private set; }
    }
}