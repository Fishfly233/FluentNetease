﻿using FluentCloudMusic.DataModels.JSONModels.Responses;
using FluentCloudMusic.Services;
using FluentCloudMusic.Utils;
using NeteaseCloudMusicApi;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public bool IsOwner
        {
            get
            {
                if (!AccountService.UserProfile.HasLogin) return false;
                return Creator.UserId == AccountService.UserProfile.UserId;
            }
        }

        public async Task<bool> AddAsync(Song song)
        {
            var param = new Dictionary<string, object>()
            {
                { "op", "add" },
                { "pid", Id },
                { "tracks", song.Id }
            };
            var jsonResult = await App.API.RequestAsync(CloudMusicApiProviders.PlaylistTracks, param);
            var result = jsonResult.ToObject<PlaylistTracksResponse>(JsonUtil.Serializer);
            return result.Code == 200;
        }

        public async Task<bool> DeleteAsync(Song song)
        {
            var param = new Dictionary<string, object>()
            {
                { "op", "del" },
                { "pid", Id },
                { "tracks", song.Id }
            };
            var jsonResult = await App.API.RequestAsync(CloudMusicApiProviders.PlaylistTracks, param);
            var result = jsonResult.ToObject<PlaylistTracksResponse>(JsonUtil.Serializer);
            return result.Code == 200;
        }
    }

    public class TrackId
    {
        public string Id { get; set; }
    }
}
