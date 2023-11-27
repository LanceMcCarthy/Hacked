using CommonHelpers.Common;

namespace Hacked.Core.Models;

public class User : BindableBase
{
    private string id;
    private string displayName;
    private bool hasNewBreaches;
    private string[] addresses;
    private string pushChannelId;

    public string Id
    {
        get => id;
        set => SetProperty(ref id, value);
    }

    public string DisplayName
    {
        get => displayName;
        set => SetProperty(ref displayName, value);
    }

    public bool HasNewBreaches
    {
        get => hasNewBreaches;
        set => SetProperty(ref hasNewBreaches, value);
    }

    public string[] Addresses
    {
        get => addresses;
        set => SetProperty(ref addresses, value);
    }

    public string PushChannelId
    {
        get => pushChannelId;
        set => SetProperty(ref pushChannelId, value);
    }
}
