namespace DbToys.Services;

public interface ILoadingService
{
    public void Active(string text = null);
    public void Dismiss();
    public Action<bool, string> LoadingRequested { get; set; }
}

public class LoadingService : ILoadingService
{
    public void Active(string text = null)
    {
        LoadingRequested(true, text);
    }

    public void Dismiss()
    {
        LoadingRequested(false, null);
    }

    public Action<bool, string> LoadingRequested { get; set; }
}