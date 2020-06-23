using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommonHelpers.Common;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Helpers;
using Microsoft.Services.Store.Engagement;
using Newtonsoft.Json.Linq;
using VungleSDK;
using VungleSDK.UI;

namespace Hacked.ViewModels
{
    public class KudosViewModel : ViewModelBase
    {
        private StoreContext context;
        private Visibility feedbackHubButtonVisibility;

        private readonly VungleAd vungleSdk;
        private const string VungleAppId = "5e347706c28ba7001748f549";
        private const string VungleKudoPlacementId = "KUDOSAD-0259168";
        private const string VungleApiEndpoint = "https://ads.api.vungle.com";

        public KudosViewModel()
        {
            KudosCollection.Add(new Kudos { Title = "Watch a Video Ad", Price = "Free", ImageUrl = "/Images/VideoAd_Colored.png" });
            KudosCollection.Add(new Kudos { Title = "Store Rating", Price = "Free", ImageUrl = "/Images/4starStar_Colored.png" });
            KudosCollection.Add(new Kudos { Title = "Remove Ads", StoreId = StoreIds.CoverApiFeeKudoStoreId, Price = "$0.99", ImageUrl = "/Images/RemoveAdKudo_Colored.png" });
            KudosCollection.Add(new Kudos { Title = "Be a Hacked Champion", StoreId = StoreIds.RecurringKudos1StoreId, Price = "$1.09 a month", ImageUrl = "/Images/RecurringKudo_Colored.png" });
            KudosCollection.Add(new Kudos { Title = "Coffee", StoreId = StoreIds.CoffeeKudoStoreId, Price = "$2.49", ImageUrl = "/Images/CoffeeKudo_Colored.png" });
            KudosCollection.Add(new Kudos { Title = "Cover API Fee", StoreId = StoreIds.CoverApiFeeKudoStoreId, Price = "$3.99", ImageUrl = "/Images/APIFeeKudo_Colored.png" });
            KudosCollection.Add(new Kudos { Title = "Lunch", StoreId = StoreIds.LunchKudoStoreId, Price = "$4.89", ImageUrl = "/Images/LunchKudo_Colored.png" });
            KudosCollection.Add(new Kudos { Title = "Dinner", StoreId = StoreIds.DinnerKudoStoreId, Price = "$19.49", ImageUrl = "/Images/DinnerKudo_Colored.png" });

            var sdkConfig = new VungleSDKConfig { ApiEndpoint = new Uri(VungleApiEndpoint) };
            vungleSdk = AdFactory.GetInstance(VungleAppId, sdkConfig);
            vungleSdk.OnInitCompleted += VungleSdk_OnInitCompleted;
            vungleSdk.Diagnostic += VungleSdk_Diagnostic;
            vungleSdk.OnAdPlayableChanged += VungleSdkOnAdPlayableChanged;
        }

        public ObservableCollection<Kudos> KudosCollection { get; set; } = new ObservableCollection<Kudos>();

        public Visibility FeedbackHubButtonVisibility
        {
            get => feedbackHubButtonVisibility;
            set => SetProperty(ref feedbackHubButtonVisibility, value);
        }

        public async void KudosGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(e.ClickedItem is Kudos kudo)) return;

            if (ApiInformation.IsTypePresent("Microsoft.Services.Store.Engagement.StoreServicesCustomEventLogger"))
                StoreServicesCustomEventLogger.GetDefault().Log($"{kudo.Title} Kudos Item Selected");

            if (!string.IsNullOrEmpty(kudo.StoreId))
            {
                await PurchaseKudosAsync(kudo.StoreId);
            }

            if (kudo.Title == "Store Rating")
            {
                await ShowRatingReviewDialog();
            }

            if (kudo.Title == "Watch a Video Ad")
            {
                // Wait for ad to be ready
                if (kudo.IsBusy)
                {
                    await new MessageDialog("Ad is being fetched right now, wait for busy indicator disappear and try again.").ShowAsync();
                    return;
                }

                await vungleSdk.PlayAdAsync(new AdConfig{Placement = VungleKudoPlacementId }, VungleKudoPlacementId);
            }
        }

        public async Task ShowRatingReviewDialog()
        {
            try
            {
                IsBusy = true;
                IsBusyMessage = "rating and review in progress (you should see a separate window)...";

                var result = await StoreRequestHelper.SendRequestAsync(StoreContext.GetDefault(), 16, "");

                IsBusyMessage = "action complete, reviewing result...";

                if (result.ExtendedError != null)
                    return;

                var jsonObject = JObject.Parse(result.Response);
                var status = jsonObject.SelectToken("status")?.ToString();

                IsBusyMessage = "action complete, showing result...";

                if (status == "success")
                {
                    await new MessageDialog("Thank you for taking the time to leave a rating! If you left 3 stars or lower, please let me know how I can improve the app (go to About page).", "Success").ShowAsync();
                }
                else if (status == "aborted")
                {
                    var md = new MessageDialog("If you prefer not to leave a bad rating but still want to provide feedback, click the email button below. I work hard to make sure you have a great app experience and would love to hear from you.", "Review Aborted");

                    md.Commands.Add(new UICommand("send email"));
                    md.Commands.Add(new UICommand("not now"));

                    var mdResult = await md.ShowAsync();

                    if (mdResult.Label == "send email")
                    {
                        var uri = new Uri($"mailto:awesome.apps@outlook.com?subject=Hacked%20Feedback&body=[write%20message%20here]", UriKind.Absolute);
                        
                        await Launcher.LaunchUriAsync(uri, new LauncherOptions
                        {
                            DesiredRemainingView = ViewSizePreference.UseHalf,
                            DisplayApplicationPicker = true,
                            PreferredApplicationPackageFamilyName = "microsoft.windowscommunicationsapps_8wekyb3d8bbwe",
                            PreferredApplicationDisplayName = "Mail"
                        });
                    }
                }
                else
                {
                    await new MessageDialog($"The rating or review did not complete, here's what Windows had to say: {jsonObject.SelectToken("status")}.\r\n\nIf you meant to leave a review, try again. If this keeps happening, contact us and share the error code above.", "Rating or Review was not successful").ShowAsync();
                }
            }
            catch (Exception ex)
            {
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("ShowRatingReviewDialog", ex);
            }
            finally
            {
                IsBusy = false;
                IsBusyMessage = "";
            }
        }

        public async Task PurchaseKudosAsync(string storeId)
        {
            try
            {
                IsBusy = true;
                IsBusyMessage = "in-app purchase in progress (you should see a separate window)...";

                if (context == null)
                    context = StoreContext.GetDefault();

                var result = await context.RequestPurchaseAsync(storeId);

                IsBusyMessage = "action complete, reviewing result...";

                var extendedError = "";

                if (result.ExtendedError != null)
                    extendedError = result.ExtendedError.Message;

                var resultMessage = "";

                switch (result.Status)
                {
                    case StorePurchaseStatus.AlreadyPurchased:
                        resultMessage = "You have already purchased this kudos, thank you!";
                        break;
                    case StorePurchaseStatus.Succeeded:
                        resultMessage = "Kudos provided! Thank you for your support and help in keeping this app free.";
                        break;
                    case StorePurchaseStatus.NotPurchased:
                        resultMessage = "Kudos were not purchased. Don't worry, you were not charged for peeking ;)";
                        break;
                    case StorePurchaseStatus.NetworkError:
                        resultMessage = "The purchase was unsuccessful due to a network error.\r\n\nError:\r\n" + extendedError;
                        break;
                    case StorePurchaseStatus.ServerError:
                        resultMessage = "The purchase was unsuccessful due to a server error.\r\n\nError:\r\n" + extendedError;
                        break;
                    default:
                        resultMessage = "The purchase was unsuccessful due to an unknown error.\r\n\nError:\r\n" + extendedError;
                        break;
                }

                IsBusyMessage = "action complete, showing result...";

                await new MessageDialog(resultMessage).ShowAsync();
            }
            catch (Exception ex)
            {
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("ShowRatingReviewDialog", ex);
            }
            finally
            {
                IsBusy = false;
                IsBusyMessage = "";
            }
        }

        private void RefreshAd()
        {
            var kudo = KudosCollection.FirstOrDefault(a => a.Title == "Video Ad");
            if (kudo != null) kudo.IsBusy = true;
        }

        #region Navigation

        public async void OnLoaded(object sender, RoutedEventArgs e)
        {
            FeedbackHubButtonVisibility = StoreServicesFeedbackLauncher.IsSupported()
                ? Visibility.Visible
                : Visibility.Collapsed;

            // get ad ready
            RefreshAd();

        }

        public async void OnUnloaded(object sender, RoutedEventArgs e)
        {


        }

        #endregion

        

        // A better option might be to make this Ad AutoCached
        private void VungleSdk_OnInitCompleted(object sender, ConfigEventArgs e)
        {
            try
            {
                vungleSdk.LoadAd(VungleKudoPlacementId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private async void VungleSdkOnAdPlayableChanged(object sender, AdPlayableEventArgs e)
        {
            Trace.WriteLine($"Ad Changed : {e.Placement}");

            if (VungleKudoPlacementId.Equals(e.Placement))
            {
                if (e.AdPlayable)
                {
                    await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        try
                        {
                            if (vungleSdk.IsAdPlayable(e.Placement))
                            {
                                KudosCollection.FirstOrDefault().IsBusy = false;
                            }
                            else
                            {
                                // Maybe we got a "sleep" code.  Let's try to Load Ad Again
                                vungleSdk.LoadAd(e.Placement);

                                KudosCollection.FirstOrDefault().IsBusy = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }
                    });

                    // Only playing ad via user click in PlayAdButton_OnClick.
                }
                else
                {
                    vungleSdk.LoadAd(VungleKudoPlacementId);

                    await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        KudosCollection.FirstOrDefault().IsBusy = true;
                    });
                }
            }
        }

        public async void VungleAd2_Start(object sender, AdEventArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                (sender as VungleAdControl).IsHitTestVisible = true;
            });
        }

        public async void VungleAd2_End(object sender, AdEndEventArgs e)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                (sender as VungleAdControl).IsHitTestVisible = false;
            });
        }

        private void VungleSdk_Diagnostic(object sender, DiagnosticLogEvent e)
        {
            if (e.Message != null && (e.Message.ToLower().Contains("exception") || e.Message.ToLower().Contains("error")))
            {
                Trace.WriteLine(e.Message);
            }
        }

    }
}
