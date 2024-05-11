using CommonHelpers.Common;

namespace Hacked.Core.Models;

public class PendingAccount : BindableBase
{
    private string address;
    private bool addSuccessful;
    private bool isFocused = true;

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
}