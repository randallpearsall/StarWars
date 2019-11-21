﻿using Newtonsoft.Json;

namespace StarWars
{
    public class GitHubRelease
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "published_at")]
        public string PublishedAt { get; set; }
    }
}
