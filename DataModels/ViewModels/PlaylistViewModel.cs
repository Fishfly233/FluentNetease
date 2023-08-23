﻿using FluentCloudMusic.DataModels.JSONModels;
using System.ComponentModel;

namespace FluentCloudMusic.DataModels.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Playlist Source
        {
            set
            {
                _Source = value;
                Notify(nameof(Id));
                Notify(nameof(Name));
                Notify(nameof(Description));
                Notify(nameof(ImageUrl));
            }
        }
        public string Id => _Source != null ? _Source.Id : string.Empty;
        public string Name => _Source != null ? _Source.Name : string.Empty;
        public string Description => _Source != null ? _Source.Description : string.Empty;
        public string ImageUrl => _Source != null ? _Source.ImageUrl : "ms-appx:///Assets/LargeTile.scale-400.png";

        private Playlist _Source;

        private void Notify( string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}