﻿using FluentNetease.Classes;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FluentNetease.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DiscoverPage : Page
    {
        public readonly ObservableCollection<Playlist> DailyRecommendPlaylists;
        public readonly ObservableCollection<Song> DailyRecommendSongs;
        public DiscoverPage()
        {
            DailyRecommendPlaylists = new ObservableCollection<Playlist>();
            DailyRecommendSongs = new ObservableCollection<Song>();

            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadPageContent();
        }

        private void PlaylistItem_Click(object sender, ItemClickEventArgs e)
        {
            /*
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ForwardConnectedAnimation", (Windows.UI.Xaml.UIElement)sender);
            MainPage.FRAME.Navigate(typeof(PlaylistPage), (Playlist)e.ClickedItem, new SuppressNavigationTransitionInfo());
            */
            MainPage.FRAME.Navigate(typeof(PlaylistPage), (Playlist)e.ClickedItem, null);
        }

        private async void LoadPageContent()
        {
            DailyRecommendPlaylists.Clear();
            DailyRecommendSongs.Clear();

            var playlists = await Network.GetDailyRecommendPlaylistsAsync();
            var songs = await Network.GetDailyRecommendSongsAsync();

            playlists?.ForEach(playlist => DailyRecommendPlaylists.Add(playlist));
            songs?.GetRange(0, 5).ForEach(song => DailyRecommendSongs.Add(song));
        }
    }
}