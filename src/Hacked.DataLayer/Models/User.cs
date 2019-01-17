using System.Runtime.Serialization;
using CommonHelpers.Common;

namespace Hacked.DataLayer.Models
{
    [DataContract]
    public class User : BindableBase
    {
        private string id;
        private string displayName;
        private bool hasNewBreaches;
        private string[] addresses;
        private string pushChannelId;

        [DataMember]
        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        [DataMember]
        public string DisplayName
        {
            get => displayName;
            set => SetProperty(ref displayName, value);
        }

        [DataMember]
        public bool HasNewBreaches
        {
            get => hasNewBreaches;
            set => SetProperty(ref hasNewBreaches, value);
        }

        [DataMember]
        public string[] Addresses
        {
            get => addresses;
            set => SetProperty(ref addresses, value);
        }

        [DataMember]
        public string PushChannelId
        {
            get => pushChannelId;
            set => SetProperty(ref pushChannelId, value);
        }
    }
}
