using CommonHelpers.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace Hacked.Core.Models;

public class MonitoredAccount : BindableBase
{
    private string _userId = string.Empty;
    private string _id = string.Empty;
    private bool _isSelected;
    private string _address = string.Empty;
    private DateTime _lastUpdated;
    private ObservableCollection<Breach> _breaches = new();
    private bool _isUpdating;
    private int _newBreachCount;
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
        get => _breaches;
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
    public int NewBreachCount
    {
        get
        {
            if (_newBreachCount == 0)
            {
                _newBreachCount = Breaches.Count(b => b.IsNew);
            }

            return _newBreachCount;
        }
        set => SetProperty(ref _newBreachCount, value);
    }

    [JsonIgnore]
    public bool HasNewBreaches
    {
        get => _hasNewBreaches = NewBreachCount > 0;
        set => SetProperty(ref _hasNewBreaches, value);
    }
}
