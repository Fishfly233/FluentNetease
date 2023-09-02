﻿using FluentCloudMusic.Services;
using Windows.UI.Xaml.Controls;

namespace FluentCloudMusic.Pages
{
    public sealed partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        private void LogoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _ = AccountService.LogoutAsync();
        }
    }
}
