using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using CommonHelpers.Common;

namespace Hacked.DataLayer.Models
{
    [DataContract]
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
        
        [DataMember]
        public string UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        [DataMember]
        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        [DataMember]
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        [DataMember]
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        [DataMember]
        public DateTime LastUpdated
        {
            get => lastUpdated;
            set => SetProperty(ref lastUpdated, value);
        }

        [DataMember]
        public ObservableCollection<Breach> Breaches
        {
            get => breaches ?? ( breaches = new ObservableCollection<Breach>());
            set => SetProperty(ref breaches, value);
        }

        public bool IsUpdating
        {
            get => isUpdating;
            set => SetProperty(ref isUpdating, value);
        }

        public int NewBreachCount => breaches.Count(a => a.IsNew);

        [DataMember]
        public bool HasNewBreaches
        {
            get
            {
                hasNewBreaches = NewBreachCount > 0;
                return hasNewBreaches;
            }
            set { hasNewBreaches = value; OnPropertyChanged();}
        }
    }
}
