﻿using FluentCloudMusic.Controls;
using FluentCloudMusic.DataModels;
using FluentCloudMusic.DataModels.JSONModels;
using FluentCloudMusic.Pages;
using FluentCloudMusic.Services;
using FluentCloudMusic.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

namespace FluentCloudMusic
{
    public sealed partial class MainPage : Page
    {
        private static readonly Dictionary<string, Type> NavButtons = new Dictionary<string, Type>
        {
            { "ItemDiscover"       , typeof(DiscoverPage)       },
            { "ItemRecommendSongs" , typeof(RecommendSongsPage) },
            { "ItemHistory"        , typeof(HistoryPage)        },
            { "ItemDownloads"      , typeof(DownloadsPage)      },
            { "ItemCloud"          , typeof(CloudPage)          },
            { "ItemFavorites"      , typeof(FavoritesPage)      },
            { "ItemPodcasts"       , typeof(PodcastsPage)       },
            { "ItemLogin"          , typeof(LoginPage)          },
            { "ItemAccount"        , typeof(AccountPage)        },
            { "ItemSettings"       , typeof(SettingsPage)       },
        };

        public static MusicPlayerControl Player;

        private static new Frame Frame;

        public readonly ObservableCollection<muxc.NavigationViewItem> CreatedPlaylistButtons;

        public readonly ObservableCollection<muxc.NavigationViewItem> BookmarkedPlaylistButtons;

        public MainPage()
        {
            CreatedPlaylistButtons = new ObservableCollection<muxc.NavigationViewItem>();
            BookmarkedPlaylistButtons = new ObservableCollection<muxc.NavigationViewItem>();

            InitializeComponent();

            Player = MusicPlayer;
            Frame = ContentFrame;
            AccountService.Login += OnLogin;
            AccountService.Logout += OnLogout;
            MainNav.SelectedItem = ItemDiscover;

            _ = AccountService.CheckLoginStatusAsync();
        }

        public static bool Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride = null)
        {
            if (Frame == null) return false;
            return Frame.Navigate(sourcePageType, parameter, infoOverride);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            AccountService.Login -= OnLogin;
            AccountService.Logout -= OnLogout;
            Player.Dispose();
        }

        /// <summary>
        /// 获取用户的歌单并添加到左侧导航栏内
        /// </summary>
        private async void GeneratePlaylistButtons()
        {
            var playlists = await AccountService.UserProfile.GetPlaylistsAsync();

            CreatedPlaylistButtons.Clear();
            BookmarkedPlaylistButtons.Clear();

            playlists?.ForEach(playlist =>
            {
                var item = new muxc.NavigationViewItem
                {
                    Name = "ItemPlaylist",
                    Tag = playlist,
                    Content = new TextBlock()
                    {
                        Text = playlist.Name,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    },
                    Icon = playlist.IsPrivate ?
                           new FontIcon() { FontFamily = new FontFamily("Segoe Fluent Icons"), Glyph = "\xe72e" } :
                           new SymbolIcon() { Symbol = Symbol.MusicInfo } as IconElement
                };
                ToolTipService.SetToolTip(item, playlist.Name);

                if (playlist.IsOwner)
                    CreatedPlaylistButtons.Add(item);
                else
                    BookmarkedPlaylistButtons.Add(item);
            });
        }

        private object FindNavigationItem(Type destPageType, object navigationParameter)
        {
            if (NavButtons.ContainsValue(destPageType))
            {
                return MainNav.MenuItems
                    .Concat(MainNav.FooterMenuItems)
                    .OfType<muxc.NavigationViewItem>()
                    .First(item =>
                    {
                        NavButtons.TryGetValue(item.Name, out Type itemPageType);
                        return itemPageType == destPageType;
                    });
            }
            else
            {
                // Find from dynamic generated buttons
                if (destPageType != typeof(PlaylistPage)) return null;

                return CreatedPlaylistButtons
                    .Concat(BookmarkedPlaylistButtons)
                    .FirstOrDefault(button => ((Playlist)button.Tag).Id == ((Playlist)navigationParameter).Id);
            }
        }

        private void OnLogin(Profile profile)
        {
            GeneratePlaylistButtons();
            ContentFrame.Navigate(typeof(DiscoverPage), null);
            ContentFrame.BackStack.Clear();
        }

        private void OnLogout()
        {
            ContentFrame.Navigate(typeof(LoginPage), null);
            ContentFrame.BackStack.Clear();
        }

        private void MainNav_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
        {
            ContentFrame.GoBack();
        }

        private void NavSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender.Text == string.Empty) return;

            var request = new SearchRequest(sender.Text);
            if (ContentFrame.CurrentSourcePageType == typeof(SearchPage))
                (ContentFrame.Content as SearchPage).Search(request);
            else
                ContentFrame.Navigate(typeof(SearchPage), request);
        }

        /// <summary>
        /// 负责处理导航事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MainNav_SelectionChanged(muxc.NavigationView sender, muxc.NavigationViewSelectionChangedEventArgs args)
        {
            var item = args.SelectedItemContainer;
            if (item == null) return;

            if (item.Name == "ItemPlaylist")
                ContentFrame.Navigate(typeof(PlaylistPage), item.Tag, args.RecommendedNavigationTransitionInfo);
            else
                ContentFrame.Navigate(NavButtons[item.Name], item.Tag, args.RecommendedNavigationTransitionInfo);
        }

        private void ItemCreatePlaylist_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 导航后更新MainNav.Header与MainNav.SelectedItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // SourcePageType should not be null
            if (e.SourcePageType == null) return;

            MainNav.SelectedItem = FindNavigationItem(e.SourcePageType, e.Parameter);
            HeaderText.Text = ResourceUtil.Get($"/Headers/{e.SourcePageType.Name}");
        }
    }
}