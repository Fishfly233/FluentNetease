﻿using Newtonsoft.Json;

namespace FluentCloudMusic.DataModels.JSONModels.Responses
{

    public class RecommendResourcesResponse : BaseResponse
    {
        [JsonProperty("recommend")]
        public Playlist[] Playlists { get; set; }
    }
}
