using CommonHelpers.Common;
using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Services.Interfaces;

namespace Hacked.Maui.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly IAccountsService _accountsService;
    private bool _refreshDuringImport = true;

    public SettingsViewModel(IAccountsService accountsService)
    {
        _accountsService = accountsService;

        ImportCommand = new Command(ImportAccounts);
        ExportCommand = new Command(ExportAccounts);
    }

    public bool RefreshDuringImport
    {
        get => _refreshDuringImport;
        set => SetProperty(ref _refreshDuringImport, value);
    }

    public Command ImportCommand { get; set; }

    public Command ExportCommand { get; set; }

    private async void ImportAccounts()
    {
        IsBusy = true;
        IsBusyMessage = RefreshDuringImport? "Importing and refreshing..." : "Importing...";

        var result = await _accountsService.ImportBackupAsync(RefreshDuringImport);

        if (result.Item1)
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterQuestion
            {
                Title = "Import successful!",
                Message = "Any non-duplicate accounts have been added and will be visible on your Monitored Accounts page.",
                Cancel = string.Empty
            });
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterError
            {
                Caller = "Import Failed",
                Exception = new Exception(result.Item2)
            });
        }

        IsBusy = false;
        IsBusyMessage = string.Empty;
    }

    private async void ExportAccounts()
    {
        var result = await _accountsService.ExportBackupAsync();

        if (result.Item1)
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterError
            {
                Caller = "Export Failed",
                Exception = new Exception(result.Item2)
            });
        }
        else
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterAlert
            {
                Message = "Export Successful!"
            });
        }
    }
}