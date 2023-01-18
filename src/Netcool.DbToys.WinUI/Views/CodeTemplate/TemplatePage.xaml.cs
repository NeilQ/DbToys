using System.Text.Json;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Netcool.DbToys.WinUI.ViewModels.CodeTemplate;

namespace Netcool.DbToys.WinUI.Views.CodeTemplate;

public sealed partial class TemplatePage : Page
{
    public TemplateViewModel ViewModel { get; }

    public bool IsEditorLoaded { get; set; }

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
         * Knows issues
         * - WinUI3 WebView2 mouse stop working while keyboard is still working.
         *  https://github.com/MicrosoftEdge/WebView2Feedback/issues/3003#issuecomment-1385402043
         * - There is no way to initialise environment in WinUI3 at the moment.
         */
        //CoreWebView2Environment.CreateAsync()
        //var webEnv = await CoreWebView2Environment.CreateAsync(null, FileSystemHelper.GetDbToysAppDataFolder());
        //var webEnv = await CoreWebView2Environment.CreateWithOptionsAsync(null, FileSystemHelper.GetDbToysAppDataFolder(), new CoreWebView2EnvironmentOptions());
        //WebView2.CoreWebView2.Environment.UserDataFolder;

        //await WebView2.EnsureCoreWebView2Async(_webEnv);
        //Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", FileSystemHelper.GetDbToysAppDataFolder());
        await WebView2.EnsureCoreWebView2Async();

        WebView2.CoreWebView2.SetVirtualHostNameToFolderMapping("monaco-editor", "Assets/Monaco", CoreWebView2HostResourceAccessKind.Allow);
        WebView2.Source = new Uri("http://monaco-editor/monaco.html");
        WebView2.CoreWebView2.AddWebResourceRequestedFilter("http://*", CoreWebView2WebResourceContext.All);
        WebView2.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
    }

    private async void OnEditorLoaded()
    {
        IsEditorLoaded = true;
        var text = await ViewModel.ReadTextAsync();
        if (!string.IsNullOrEmpty(text))
        {
            PostCode(text);
        }
    }

    private void CoreWebView2_WebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        var json = JsonDocument.Parse(args.WebMessageAsJson);
        var type = json.RootElement.GetProperty("Type").GetString();
        switch (type)
        {
            case "EditorLoaded":
                OnEditorLoaded();
                break;
            case "Save":
                var text = json.RootElement.GetProperty("Text").GetString();
                ViewModel.SaveText(text);
                break;
        }
    }
}

