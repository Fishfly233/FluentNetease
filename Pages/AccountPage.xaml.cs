﻿using FluentCloudMusic.Services;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FluentCloudMusic.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        public AccountPage()
        {
            this.InitializeComponent();
        }

        private void LogoutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AccountService.LogoutAsync();
        }
    }
}
