using CommonHelpers.Common;
using CommunityToolkit.Mvvm.Messaging;
using Hacked.Core.Common;
using Hacked.Maui.Services;

namespace Hacked.Maui.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly AccountsService _accountsService;

    public SettingsViewModel(AccountsService accountsService)
    {
        _accountsService = accountsService;

        ImportCommand = new Command(ImportAccounts);
        ExportCommand = new Command(ExportAccounts);
    }

    public string AppVersion => "1.0";

    public Command ImportCommand { get; set; }
    
    public Command ExportCommand { get; set; }

    private async void ImportAccounts()
    {
        var result = await _accountsService.ImportBackupAsync();

        if (!result.Item1)
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterError
            {
                Caller = "Import Failed", 
                Exception = new Exception(result.Item2)
            });
        }
    }

    private async void ExportAccounts()
    {
        var result = await _accountsService.ExportBackupAsync();

        if (!result.Item1)
        {
            WeakReferenceMessenger.Default.Send(new MessagingCenterError
            {
                Caller = "Export Failed", 
                Exception = new Exception(result.Item2)
            });
        }
    }
}