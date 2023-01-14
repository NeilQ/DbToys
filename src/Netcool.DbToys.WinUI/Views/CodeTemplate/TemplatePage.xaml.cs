using System.Reflection;
using System.Text.Json;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

namespace Netcool.DbToys.WinUI.Views.CodeTemplate;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TemplatePage : Page
{
    public TemplateViewModel ViewModel { get; }
    public bool IsReady { get; set; }

    public TemplatePage()
    {
        ViewModel = App.GetService<TemplateViewModel>();
        InitializeComponent();
        InitWebView2();
    }

    public void PostCode(string text)
    {
        WebView2.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new { Type = "UpdateText", Text = text }));
    }

    public async void InitWebView2()
    {
        /*
         * There is no way to initialise environment in WinUI3 at the moment.
         */
        //CoreWebView2Environment.CreateAsync()
        //var webEnv = await CoreWebView2Environment.CreateAsync(null, FileSystemHelper.GetDbToysAppDataFolder());
        //var webEnv = await CoreWebView2Environment.CreateWithOptionsAsync(null, FileSystemHelper.GetDbToysAppDataFolder(), new CoreWebView2EnvironmentOptions());
        //WebView2.CoreWebView2.Environment.UserDataFolder;

        //await WebView2.EnsureCoreWebView2Async(_webEnv);
        await WebView2.EnsureCoreWebView2Async();
        //Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", FileSystemHelper.GetDbToysAppDataFolder());

        WebView2.CoreWebView2.SetVirtualHostNameToFolderMapping("monaco-editor", "Assets/Monaco", CoreWebView2HostResourceAccessKind.Allow);
        WebView2.Source = new Uri("http://monaco-editor/monaco.html");
        WebView2.CoreWebView2.AddWebResourceRequestedFilter("http://*", CoreWebView2WebResourceContext.All);
        WebView2.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
    }

    private void OnReady()
    {
        PostCode(ViewModel.Text);
    }

    private void CoreWebView2_WebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        IsReady = true;
        var json = JsonDocument.Parse(args.WebMessageAsJson);
        var type = json.RootElement.GetProperty("Type").GetString();
        switch (type)
        {
            case "EditorLoaded":
                OnReady();
                break;
            case "TextChanged":
                ViewModel.Text = json.RootElement.GetProperty("Text").GetString();
                break;
        }
    }
}

