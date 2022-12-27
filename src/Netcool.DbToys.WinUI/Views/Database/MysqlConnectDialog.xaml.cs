// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Netcool.DbToys.WinUI.ViewModels.Database;
namespace Netcool.DbToys.WinUI.Views.Database;

public sealed partial class MysqlConnectDialog
{
    public MysqlConnectViewModel ViewModel { get; }
    public MysqlConnectDialog()
    {
        ViewModel = App.GetService<MysqlConnectViewModel>();
        ViewModel.HasError = false;
        ViewModel.SchemaReader = null;
        InitializeComponent();
    }
    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.Password = ((PasswordBox)sender).Password;
    }
}