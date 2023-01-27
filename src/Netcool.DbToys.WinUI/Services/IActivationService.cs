namespace Netcool.DbToys.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
