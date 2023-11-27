using CommonHelpers.Common;
using Hacked.Maui.Services;

namespace Hacked.Maui.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly AccountsService accountsService;

    public SettingsViewModel(AccountsService accountsService)
    {
        this.accountsService = accountsService;

        ImportCommand = new Command(ImportAccounts);
        ExportCommand = new Command(ExportAccounts);
    }

    public string AppVersion => "1.0";

    public Command ImportCommand { get; set; }
    
    public Command ExportCommand { get; set; }

    private async void ImportAccounts()
    {
        await accountsService.ImportBackupAsync();
    }

    private async void ExportAccounts()
    {
        await accountsService.ExportBackupAsync();
    }
}