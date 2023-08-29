﻿using FluentCloudMusic.DataModels.JSONModels;
using FluentCloudMusic.DataModels.JSONModels.Responses;
using FluentCloudMusic.Utils;
using NeteaseCloudMusicApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace FluentCloudMusic.Services
{
    public static class SongService
    {
        public static async Task<List<Song>> GetDailyRecommendSongsAsync()
        {
            var jsonResult = await App.API.RequestAsync(CloudMusicApiProviders.RecommendSongs);
            var result = jsonResult.ToObject<RecommendSongsResponse>(JsonUtil.Serializer);
            return result.Code == 200 ? result.Data.DailySongs.ToList() : new List<Song>();
        }

        public static async Task<(bool isSuccess, MediaPlaybackItem result)> GetNeteaseSongUrl(ISong song)
        {
            if (song == null || !song.HasCopyright) return (false, null);

            var parameters = new Dictionary<string, object> { { "id", song.Id }, { "level", "standard" } };

            var jsonResult = await App.API.RequestAsync(CloudMusicApiProviders.SongUrlV1, parameters);
            var code = jsonResult["code"].Value<int>();

            if (code != 200 || jsonResult["data"].First["url"].ToString() == string.Empty) return (false, null);

            var result = new MediaPlaybackItem(MediaSource.CreateFromUri(new Uri(jsonResult["data"].First["url"].ToString())));

            var metadata = result.GetDisplayProperties();
            metadata.Type = Windows.Media.MediaPlaybackType.Music;
            metadata.MusicProperties.Title = $"{song.Name}{song.Description}";
            metadata.MusicProperties.Artist = song.ArtistName;
            metadata.MusicProperties.AlbumTitle = song.AlbumName;
            metadata.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(song.ImageUrl));
            result.ApplyDisplayProperties(metadata);

            return (true, result);
        }
    }
}
