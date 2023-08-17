﻿using FluentCloudMusic.Classes;
using FluentCloudMusic.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace FluentCloudMusic.Controls
{
    public sealed partial class MusicListView : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<Song>), typeof(MusicListView), new PropertyMetadata(new ObservableCollection<Song>()));
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(UIElement), typeof(MusicListView), new PropertyMetadata(null));
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(UIElement), typeof(MusicListView), new PropertyMetadata(null));
        public static readonly DependencyProperty IsArtistButtonEnabledProperty =
            DependencyProperty.Register("IsArtistButtonEnabled", typeof(bool), typeof(MusicListView), new PropertyMetadata(true));
        public static readonly DependencyProperty IsAlbumButtonEnabledProperty =
            DependencyProperty.Register("IsAlbumButtonEnabled", typeof(bool), typeof(MusicListView), new PropertyMetadata(true));
        public static readonly DependencyProperty IsToolBarEnabledProperty =
            DependencyProperty.Register("IsToolBarEnabled", typeof(bool), typeof(MusicListView), new PropertyMetadata(true));

        public ObservableCollection<Song> ItemsSource
        {
            get { return (ObservableCollection<Song>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public UIElement Header
        {
            get { return (UIElement)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public UIElement Footer
        {
            get { return (UIElement)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }
        public bool IsArtistButtonEnabled
        {
            get { return (bool)GetValue(IsArtistButtonEnabledProperty); }
            set { SetValue(IsArtistButtonEnabledProperty, value); }
        }
        public bool IsAlbumButtonEnabled
        {
            get { return (bool)GetValue(IsAlbumButtonEnabledProperty); }
            set { SetValue(IsAlbumButtonEnabledProperty, value); }
        }
        public bool IsToolBarEnabled
        {
            get { return (bool)GetValue(IsToolBarEnabledProperty); }
            set { SetValue(IsToolBarEnabledProperty, value); }
        }

        private readonly ObservableCollection<Song> FilteredSongs;

        public MusicListView()
        {
            FilteredSongs = new ObservableCollection<Song>();
            InitializeComponent();
        }

        public void ApplyFilter(string filter)
        {
            FilteredSongs.Clear();
            if (filter == string.Empty) foreach (var song in ItemsSource) FilteredSongs.Add(song);
            else foreach (var song in ItemsSource.Where(song => song.RelateTo(filter))) FilteredSongs.Add(song);
        }

        private void MusicNameButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Player.Play((AbstractMusic)((FrameworkElement)sender).Tag);
        }

        private void ArtistNameButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Navigate(typeof(ArtistPage), ((FrameworkElement)sender).Tag);
        }

        private void AlbumNameButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Navigate(typeof(AlbumPage), ((FrameworkElement)sender).Tag);
        }

        private void PlayAllButton_Click(object sender, RoutedEventArgs e)
        {
            var playlist = new List<AbstractMusic>();
            foreach (var song in FilteredSongs) playlist.Add(song.Music);
            MainPage.Player.Play(playlist);
        }

        private void FilterInputBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text == string.Empty) ApplyFilter(string.Empty);
        }

        private void FilterInputBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ApplyFilter(sender.Text);
        }
    }
}