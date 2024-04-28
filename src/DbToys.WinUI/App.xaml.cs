using DbToys.Activation;
using DbToys.Core;
using DbToys.Core.Excel;
using DbToys.Core.Log;
using DbToys.Services;
using DbToys.Services.Settings;
using DbToys.ViewModels;
using DbToys.ViewModels.CodeTemplate;
using DbToys.ViewModels.Dialogs;
using DbToys.Views;
using DbToys.Views.CodeTemplate;
using DbToys.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace DbToys;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((_, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<IDatabaseAccountHistory, DatabaseAccountHistory>();
            services.AddSingleton<GeneralSettingsService>();
            services.AddSingleton<UiSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ILoadingService, LoadingService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<CodeTemplateStorageService>();

            // Core Services
            services.AddTransient<IExcelService, ExcelService>();
            services.AddTransient<ICodeGenerator, CodeGenerator>();

            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddSingleton<LogPage>();
            services.AddSingleton<LogViewModel>();
            services.AddSingleton<DatabasePage>();
            services.AddSingleton<DatabaseViewModel>();
            services.AddSingleton<CodeTemplateExplorerPage>();
            services.AddSingleton<CodeTemplateExplorerViewModel>();
            services.AddTransient<PostgreSqlConnectDialog>();
            services.AddSingleton<PostgreSqlConnectViewModel>();
            services.AddTransient<MysqlConnectDialog>();
            services.AddSingleton<MysqlConnectViewModel>();
            services.AddTransient<SqlServerConnectDialog>();
            services.AddSingleton<SqlServerConnectViewModel>();
            services.AddTransient<TableDetailViewModel>();
            services.AddTransient<TemplateViewModel>();
            services.AddTransient<TemplatePage>();
            services.AddTransient<GenerateCodeDialog>();
            services.AddTransient<GenerateCodeViewModel>();
            services.AddTransient<TemplateFilenameDialog>();
            services.AddTransient<TemplateFilenameViewModel>();

            // Configuration
            //services.Configure<SettingsOptions>(context.Configuration.GetSection(nameof(SettingsOptions)));
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Logger.Error(e.Message, e.Exception);
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
