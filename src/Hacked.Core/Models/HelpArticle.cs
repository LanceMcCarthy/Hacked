using CommonHelpers.Common;

namespace Hacked.Core.Models;

public class HelpArticle : BindableBase
{
    private bool isExpanded;

    public string Title { get; set; }

    public string Summary { get; set; }

    public string ImageUrl { get; set; }

    public bool IsExpanded
    {
        get => isExpanded;
        set => SetProperty(ref isExpanded, value);
    }
}
