﻿using FluentCloudMusic.DataModels.JSONModels;
using FluentCloudMusic.DataModels.ViewModels;
using FluentCloudMusic.Services;
using FluentCloudMusic.Utils;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FluentCloudMusic.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DiscoverPage : Page
    {
        // 这串magic number会不会给人一种钦定的感觉？
        private const string DailyRecommendSongsPlaylistId = "2829896389";

        public readonly ObservableCollection<Playlist> DailyRecommendPlaylists;
        public readonly ObservableCollection<Song> DailyRecommendSongs;

        public readonly DiscoverPageViewModel ViewModel;

        public DiscoverPage()
        {
            DailyRecommendPlaylists = new ObservableCollection<Playlist>();
            DailyRecommendSongs = new ObservableCollection<Song>();
            ViewModel = new DiscoverPageViewModel();

            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AccountService.UserProfile.HasLogin) GetRecommendations();
        }

        private void PlaylistItem_Click(object sender, ItemClickEventArgs e)
        {
            var imageContainer = 
                VisualTreeUtils.FindChildByName(DailyRecommendPlaylistsGridView.ContainerFromItem(e.ClickedItem), "CoverImageContainer");
            ConnectedAnimationService
                .GetForCurrentView()
                .PrepareToAnimate("DailyRecommendPlaylistsToPlaylistPageAnimation", (UIElement)imageContainer);
            MainPage.Navigate(typeof(PlaylistPage), (Playlist)e.ClickedItem);
        }

        private async void GetRecommendations()
        {
            DailyRecommendPlaylists.Clear();
            DailyRecommendSongs.Clear();

            var playlists = await PlaylistService.GetDailyRecommendPlaylistsAsync();
            var songs = await SongService.GetDailyRecommendSongsAsync();

            playlists?.ForEach(playlist => { 
                if (playlist.Id != DailyRecommendSongsPlaylistId) DailyRecommendPlaylists.Add(playlist); 
            });
            songs?.GetRange(0, 5).ForEach(song => DailyRecommendSongs.Add(song));
            
            ViewModel.RecommendPlaylistsLoaded = true;
            ViewModel.RecommendSongsLoaded = true;
        }

        private void DailyRecommendSongsViewAllButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Navigate(typeof(PlaylistPage), new Playlist() { Id = DailyRecommendSongsPlaylistId });
        }
    }
}