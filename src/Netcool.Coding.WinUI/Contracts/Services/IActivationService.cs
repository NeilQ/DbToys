﻿namespace Netcool.Coding.WinUI.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
