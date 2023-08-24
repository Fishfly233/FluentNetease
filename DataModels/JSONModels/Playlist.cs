﻿using FluentCloudMusic.Utils;

namespace FluentCloudMusic.DataModels.JSONModels
{
    public class Playlist
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [MultipleJsonProperty("picUrl", "coverImgUrl")]
        public string ImageUrl { get; set; }

        public Profile Creator { get; set; }

        public TrackId[] TrackIds { get; set; }
    }

    public class TrackId
    {
        public string Id { get; set; }
    }
}
