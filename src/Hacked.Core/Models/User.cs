using CommonHelpers.Common;
using System;

namespace Hacked.Core.Models;

public class User : BindableBase
{
    private string id = string.Empty;
    private string displayName = string.Empty;
    private bool hasNewBreaches;
    private string[] addresses = Array.Empty<string>();
    private string pushChannelId = string.Empty;

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
