using CommonHelpers.Common;

namespace Hacked.Core.Models;

public class Kudos : BindableBase
{
    private string title = string.Empty;
    private string storeId = string.Empty;
    private string imageUrl = string.Empty;
    private string price = string.Empty;
    private KudoCategory category;

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

    public KudoCategory Category
    {
        get => category;
        set => SetProperty(ref category, value);
    }
}
