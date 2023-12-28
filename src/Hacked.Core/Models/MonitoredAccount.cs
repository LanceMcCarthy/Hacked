using CommonHelpers.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace Hacked.Core.Models;

public class MonitoredAccount : BindableBase
{
    private string _userId;
    private string _id;
    private bool _isSelected;
    private string _address;
    private DateTime _lastUpdated;
    private ObservableCollection<Breach> _breaches;
    private bool _isUpdating;
    private bool _hasNewBreaches;

    public string UserId
    {
        get => _userId;
        set => SetProperty(ref _userId, value);
    }

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public DateTime LastUpdated
    {
        get => _lastUpdated;
        set => SetProperty(ref _lastUpdated, value);
    }

    public ObservableCollection<Breach> Breaches
    {
        get => _breaches ??= new ObservableCollection<Breach>();
        set => SetProperty(ref _breaches, value);
    }

    [JsonIgnore]
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    [JsonIgnore]
    public bool IsUpdating
    {
        get => _isUpdating;
        set => SetProperty(ref _isUpdating, value);
    }

    [JsonIgnore]
    public int NewBreachCount => Breaches.Count(b => b.IsNew);

    [JsonIgnore]
    public bool HasNewBreaches
    {
        get => _hasNewBreaches = NewBreachCount > 0;
        set => SetProperty(ref _hasNewBreaches, value);
    }
}
