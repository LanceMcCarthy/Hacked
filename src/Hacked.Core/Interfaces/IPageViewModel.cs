namespace Hacked.Core.Interfaces;

public interface IPageViewModel
{
    void OnAppearing();

    bool OnBackButtonRequested();
}