using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Netcool.DbToys.Core.Log;
using Netcool.DbToys.Helpers;
using Netcool.DbToys.Services;

namespace Netcool.DbToys.ViewModels;

public class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    private int _themeIndex;
    public int ThemeIndex { get => _themeIndex; set => SetProperty(ref _themeIndex, value); }

    private ElementTheme _elementTheme;
    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    private string _appDisplayName;
    public string AppDisplayName { get => _appDisplayName; set => SetProperty(ref _appDisplayName, value); }

    private string _versionDescription;
    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    public ICommand SwitchThemeCommand { get; }
    public IRelayCommand OpenLogsCommand { get; }

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _appDisplayName = "AppDisplayName".GetLocalized();
        _versionDescription = GetVersionDescription();
        switch (_elementTheme)
        {
            case ElementTheme.Default:
                ThemeIndex = 2;
                break;
            case ElementTheme.Light:
                ThemeIndex = 0;
                break;
            case ElementTheme.Dark:
                ThemeIndex = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async param =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
        OpenLogsCommand = new RelayCommand(() =>
        {
            try
            {
                Process.Start("explorer.exe", Logger.ApplicationLogPath);
            }
            catch (Win32Exception)
            {
                // ignore
            }
        });
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ThemeIndex))
        {
            ElementTheme theme = ElementTheme.Default;
            switch (ThemeIndex)
            {
                case 0: // light
                    theme = ElementTheme.Light;
                    break;
                case 1: //dark
                    theme = ElementTheme.Dark;
                    break;
                case 2: // default
                    theme = ElementTheme.Default;
                    break;
            }

            if (theme != ElementTheme)
            {
                ElementTheme = theme;
                await _themeSelectorService.SetThemeAsync(ElementTheme);
            }
        }
    }

    private static string GetVersionDescription()
    {
        Version version;


        var buildConfiguration = "RELEASE";
#if DEBUG
        buildConfiguration = "DEBUG";
#endif


        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{version} | {buildConfiguration}";

        //return $".{version.Minor}.{version.Build}.{version.Revision}";
    }
}
