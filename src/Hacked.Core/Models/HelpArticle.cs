using CommonHelpers.Common;

namespace Hacked.Core.Models;

public class HelpArticle : BindableBase
{
    private bool isExpanded;

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsExpanded
    {
        get => isExpanded;
        set => SetProperty(ref isExpanded, value);
    }
}
