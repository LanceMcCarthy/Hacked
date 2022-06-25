using CommonHelpers.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hacked.Core.Models;

public class MonitoredAccount : BindableBase
{
    private string userId;
    private string id;
    private bool isSelected;
    private string address;
    private DateTime lastUpdated;
    private ObservableCollection<Breach> breaches;
    private bool isUpdating;
    private bool hasNewBreaches;

    public string UserId
    {
        get => userId;
        set => SetProperty(ref userId, value);
    }

    public string Id
    {
        get => id;
        set => SetProperty(ref id, value);
    }

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }

    public string Address
    {
        get => address;
        set => SetProperty(ref address, value);
    }

    public DateTime LastUpdated
    {
        get => lastUpdated;
        set => SetProperty(ref lastUpdated, value);
    }

    public ObservableCollection<Breach> Breaches
    {
        get => breaches ??= new ObservableCollection<Breach>();
        set => SetProperty(ref breaches, value);
    }

    public bool IsUpdating
    {
        get => isUpdating;
        set => SetProperty(ref isUpdating, value);
    }

    public int NewBreachCount => Breaches.Count(b => b.IsNew);

    public bool HasNewBreaches
    {
        get => hasNewBreaches = NewBreachCount > 0;
        set => SetProperty(ref hasNewBreaches, value);
    }
}
