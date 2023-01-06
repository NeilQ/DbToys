namespace Netcool.DbToys.WinUI.Services;

public interface ILoadingService
{
    public void Active();
    public void Dismiss();
    public Action<bool> LoadingRequested { get; set; }
}

public class LoadingService : ILoadingService
{
    public void Active()
    {
        LoadingRequested(true);
    }

    public void Dismiss()
    {
        LoadingRequested(false);
    }

    public Action<bool> LoadingRequested { get; set; }
}