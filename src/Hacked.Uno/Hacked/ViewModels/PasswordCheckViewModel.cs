using Hacked.Services.Interfaces;

namespace Hacked.ViewModels;

public partial class PasswordCheckViewModel : ObservableObject
{
    private readonly IPwndPasswordService _passwordService;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _result = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public PasswordCheckViewModel(IPwndPasswordService passwordService)
    {
        _passwordService = passwordService;
    }

    [RelayCommand]
    private async Task CheckPassword()
    {
        if (string.IsNullOrWhiteSpace(Password)) return;
        IsBusy = true;
        Result = string.Empty;
        try
        {
            var resultText = await _passwordService.CheckPasswordAsync(Password);
            Result = resultText;
        }
        catch (Exception ex)
        {
            Result = $"Error: {ex.Message}";
        }
        finally { IsBusy = false; }
    }
}
