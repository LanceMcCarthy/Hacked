using CommonHelpers.Common;

namespace Hacked.Core.Models;

public class PendingAccount : BindableBase
{
    private string address = string.Empty;
    private bool addSuccessful;
    private bool isFocused = true;
    private bool isLast;

    public string Address
    {
        get => address;
        set => SetProperty(ref address, value);
    }

    public bool AddSuccessful
    {
        get => addSuccessful;
        set => SetProperty(ref addSuccessful, value);
    }

    public bool IsFocused
    {
        get => isFocused;
        set => SetProperty(ref isFocused, value);
    }

    public bool IsLast
    {
        get => isLast;
        set => SetProperty(ref isLast, value);
    }
}