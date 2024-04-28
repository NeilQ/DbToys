using System.Text.Encodings.Web;
using System.Text.Json;
using DbToys.CodeEditor;
using DbToys.ViewModels.CodeTemplate;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;

namespace DbToys.Views.CodeTemplate;

public sealed partial class TemplatePage : Page
{
    public TemplateViewModel ViewModel { get; }
    public WebView2 WebView2 = new() { VerticalAlignment = VerticalAlignment.Stretch };

    public bool IsEditorLoaded { get; set; }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public TemplatePage()
    {
        ViewModel = App.GetService<TemplateViewModel>();
        InitializeComponent();
        InitWebView2();
    }

    public void CloseWebView2()
    {
        WebView2.Close();
    }

    private void PostCode(string text)
    {
        WebView2.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new { Type = "UpdateText", Text = text },
            _jsonOptions));
    }

    private void PostCompletions()
    {
        var completions = new List<CompletionItem>();
        completions.AddRange(CompletionItem.VariableCompletionItems);
        completions.AddRange(CompletionItem.CustomCompletionItems);
        completions.AddRange(CompletionItem.ScribanCompletionItems);
        WebView2.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(new
        {
            Type = "UpdateCompletions",
            Completions = completions
        }, _jsonOptions));
    }

    private async void InitWebView2()
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
        PostCompletions();
        DispatcherQueue.TryEnqueue(() =>
        {
            // add webview2 after page loaded, or it will case page flash
            // https://github.com/MicrosoftEdge/WebView2Feedback/issues/1412
            ContextArea.Children.RemoveAt(0);
            ContextArea.Children.Add(WebView2);
        });
    }

    private void CoreWebView2_WebMessageReceived(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
    {
        var json = JsonDocument.Parse(args.WebMessageAsJson);
        var type = json.RootElement.GetProperty("type").GetString();
        switch (type)
        {
            case "EditorLoaded":
                OnEditorLoaded();
                break;
            case "Save":
                var text = json.RootElement.GetProperty("text").GetString();
                ViewModel.SaveText(text);
                break;
        }
    }
}

