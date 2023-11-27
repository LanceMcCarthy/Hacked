using CommonHelpers.Common;
using Hacked.Core.Interfaces;

namespace Hacked.Core.Common;

public class PageViewModelBase : ViewModelBase, IPageViewModel
{
    public virtual void OnAppearing() {}

    public virtual bool OnBackButtonRequested() => false;
}
