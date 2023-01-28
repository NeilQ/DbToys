using System.Windows.Input;
using Windows.System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbToys.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace DbToys.ViewModels.Dialogs;

public class DynamicDialogViewModel : ObservableObject
{
    private string _title;
    public string Title { get => _title; set => SetProperty(ref _title, value); }

    private string _subTitle;
    public string SubTitle
    {
        get => _subTitle;
        set
        {
            if (SetProperty(ref _subTitle, value))
                SubTitleLoad = !string.IsNullOrWhiteSpace(value);
        }
    }

    #region Primary Button

    private string _primaryButtonText = "Ok";

    public string PrimaryButtonText { get => _primaryButtonText; set => SetProperty(ref _primaryButtonText, value); }

    private bool _isPrimaryButtonEnabled = true;

    public bool IsPrimaryButtonEnabled { get => _isPrimaryButtonEnabled; set => SetProperty(ref _isPrimaryButtonEnabled, value); }

    #endregion Primary Button

    #region Secondary Button

    private string _secondaryButtonText;

    public string SecondaryButtonText { get => _secondaryButtonText; set => SetProperty(ref _secondaryButtonText, value); }

    private bool _isSecondaryButtonEnabled;

    public bool IsSecondaryButtonEnabled { get => _isSecondaryButtonEnabled; set => SetProperty(ref _isSecondaryButtonEnabled, value); }

    #endregion Secondary Button

    #region Close Button

    private string _closeButtonText;

    public string CloseButtonText { get => _closeButtonText; set => SetProperty(ref _closeButtonText, value); }

    #endregion Close Button

    #region Commands

    public ICommand PrimaryButtonCommand { get; set; }

    public ICommand SecondaryButtonCommand { get; set; }

    public ICommand CloseButtonCommand { get; set; }

    public ICommand KeyDownCommand { get; set; }

    public ICommand DisplayControlOnLoadedCommand { get; set; }

    #endregion Commands

    private bool _subTitleLoad;
    public bool SubTitleLoad
    {
        get => _subTitleLoad;
        private set => SetProperty(ref _subTitleLoad, value);
    }

    private bool _displayControlLoad;
    public bool DisplayControlLoad
    {
        get => _displayControlLoad;
        set => SetProperty(ref _displayControlLoad, value);
    }

    /// <summary>
    /// Stores any additional data that could be written to, read from.
    /// </summary>
    public object AdditionalData { get; set; }

    private object _displayControl;

    /// <summary>
    /// The control that is dynamically displayed.
    /// </summary>
    public object DisplayControl
    {
        get => _displayControl;
        set
        {
            if (SetProperty(ref _displayControl, value))
            {
                DisplayControlLoad = value is not null;
            }
        }
    }

    #region Actions

    public Action HideDialog { get; set; }

    private Action<DynamicDialogViewModel, ContentDialogButtonClickEventArgs> _primaryButtonAction;
    public Action<DynamicDialogViewModel, ContentDialogButtonClickEventArgs> PrimaryButtonAction
    {
        get => _primaryButtonAction;
        set
        {
            if (SetProperty(ref _primaryButtonAction, value))
            {
                PrimaryButtonCommand = new RelayCommand<ContentDialogButtonClickEventArgs>((e) =>
                {
                    DialogResult = ContentDialogResult.Primary;
                    PrimaryButtonAction(this, e);
                });
            }
        }
    }

    private Action<DynamicDialogViewModel, ContentDialogButtonClickEventArgs> _secondaryButtonAction;
    public Action<DynamicDialogViewModel, ContentDialogButtonClickEventArgs> SecondaryButtonAction
    {
        get => _secondaryButtonAction;
        set
        {
            if (SetProperty(ref _secondaryButtonAction, value))
            {
                SecondaryButtonCommand = new RelayCommand<ContentDialogButtonClickEventArgs>((e) =>
                {
                    DialogResult = ContentDialogResult.Secondary;
                    SecondaryButtonAction(this, e);
                });
            }
        }
    }

    private Action<DynamicDialogViewModel, ContentDialogButtonClickEventArgs> _closeButtonAction;
    public Action<DynamicDialogViewModel, ContentDialogButtonClickEventArgs> CloseButtonAction
    {
        get => _closeButtonAction;
        set
        {
            if (SetProperty(ref _closeButtonAction, value))
            {
                CloseButtonCommand = new RelayCommand<ContentDialogButtonClickEventArgs>(e =>
                {
                    DialogResult = ContentDialogResult.None;
                    CloseButtonAction(this, e);
                });
            }
        }
    }

    private Action<DynamicDialogViewModel, KeyRoutedEventArgs> _keyDownAction;

    public Action<DynamicDialogViewModel, KeyRoutedEventArgs> KeyDownAction
    {
        get => _keyDownAction;
        set
        {
            if (SetProperty(ref _keyDownAction, value))
            {
                KeyDownCommand = new RelayCommand<KeyRoutedEventArgs>((e) =>
                {
                    DialogResult = ContentDialogResult.None;
                    KeyDownAction(this, e);
                });
            }
        }
    }

    private Action<DynamicDialogViewModel, RoutedEventArgs> _displayControlOnLoaded;

    public Action<DynamicDialogViewModel, RoutedEventArgs> DisplayControlOnLoaded
    {
        get => _displayControlOnLoaded;
        set
        {
            if (SetProperty(ref _displayControlOnLoaded, value))
            {
                DisplayControlOnLoadedCommand = new RelayCommand<RoutedEventArgs>(e =>
                {
                    DisplayControlOnLoaded(this, e);
                });
            }
        }
    }

    #endregion Actions

    public ContentDialogResult DialogResult { get; set; } = ContentDialogResult.None;

    public DynamicDialogViewModel()
    {
        PrimaryButtonAction = (_, _) => HideDialog?.Invoke();
        SecondaryButtonAction = (_, _) => HideDialog?.Invoke();
        CloseButtonAction = (_, _) => HideDialog?.Invoke();
        KeyDownAction = (_, e) =>
        {
            if (e.Key == VirtualKey.Escape)
            {
                HideDialog?.Invoke();
            }
        };
        DisplayControlOnLoaded = (vm, _) =>
        {
            var control = vm.DisplayControl as Control ?? DependencyObjectHelper.FindChild<Control>(vm.DisplayControl as DependencyObject);
            control?.Focus(FocusState.Programmatic);
        };
    }


}