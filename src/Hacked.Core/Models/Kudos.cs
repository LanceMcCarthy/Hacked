using CommonHelpers.Common;

namespace Hacked.Core.Models
{
    public class Kudos : BindableBase
    {
        private string title;
        private string storeId;
        private string imageUrl;
        private string price;
        private bool isBusy;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string StoreId
        {
            get => storeId;
            set => SetProperty(ref storeId, value);
        }

        public string ImageUrl
        {
            get => imageUrl;
            set => SetProperty(ref imageUrl, value);
        }

        public string Price
        {
            get => price;
            set => SetProperty(ref price, value);
        }

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }
    }
}
