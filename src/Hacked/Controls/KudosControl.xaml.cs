using Hacked.Core.Args;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Hacked.Helpers;
using Microsoft.Services.Store.Engagement;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hacked.Controls
{
    public sealed partial class KudosControl : UserControl
    {
        private StoreContext context;
        private const string VungleKudoPlacementId = "KUDOSAD-0259168";
        public event EventHandler<AdRequestedArgs> PlayAdRequested;

        public KudosControl()
        {
            InitializeComponent();

            Kudoses = LoadKudos();
            KudosGridView.ItemsSource = Kudoses;
        }

        #region Dependency Properties

        public static readonly DependencyProperty KudosesProperty = DependencyProperty.Register(
            "Kudoses", typeof(ObservableCollection<Kudos>), typeof(KudosControl), new PropertyMetadata(default(ObservableCollection<Kudos>)));

        public ObservableCollection<Kudos> Kudoses
        {
            get => (ObservableCollection<Kudos>) GetValue(KudosesProperty);
            set => SetValue(KudosesProperty, value);
        }

        #endregion

        #region Event handlers

        public async void KudosGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (!(e.ClickedItem is Kudos kudo)) return;

            if (ApiInformation.IsTypePresent("Microsoft.Services.Store.Engagement.StoreServicesCustomEventLogger"))
                StoreServicesCustomEventLogger.GetDefault().Log($"{kudo.Title} Kudos Item Selected");

            if (kudo.Category == KudoCategory.Consumable ||
                kudo.Category == KudoCategory.Subscription ||
                kudo.Category == KudoCategory.Durable)
            {
                await PurchaseKudosAsync(kudo.StoreId);
            }

            if (kudo.Category == KudoCategory.Free)
            {
                if (kudo.Title == "Store Rating")
                {
                    await ShowRatingReviewDialog();
                }

                if (kudo.Title == "Play Ad")
                {
                    PlayAdRequested?.Invoke(this, new AdRequestedArgs(VungleKudoPlacementId));
                }
            }
        }

        #endregion

        #region Instance methods and events

        public async Task ShowRatingReviewDialog()
        {
            try
            {
                UpdateBusyMessage("rating and review in progress (you should see a separate window)...");

                var result = await StoreRequestHelper.SendRequestAsync(StoreContext.GetDefault(), 16, "");

                UpdateBusyMessage("action complete, reviewing result...");

                if (result.ExtendedError != null)
                    return;

                var jsonObject = JObject.Parse(result.Response);
                var status = jsonObject.SelectToken("status")?.ToString();

                UpdateBusyMessage("action complete, showing result...");

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
                UpdateBusyMessage();
            }
        }

        public async Task PurchaseKudosAsync(string storeId)
        {
            try
            {
                UpdateBusyMessage("in-app purchase in progress (you should see a separate window)...");

                if (context == null)
                    context = StoreContext.GetDefault();

                var result = await context.RequestPurchaseAsync(storeId);

                UpdateBusyMessage("action complete, reviewing result...");

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

                UpdateBusyMessage("action complete, showing result...");

                await new MessageDialog(resultMessage).ShowAsync();
            }
            catch (Exception ex)
            {
                DisplayMessageHelpers.ShowExceptionMessageOnUiThread("ShowRatingReviewDialog", ex);
            }
            finally
            {
                UpdateBusyMessage();
            }
        }

        private void UpdateBusyMessage(string message = "")
        {
            if (!string.IsNullOrEmpty(message))
            {
                KudosBusyIndicator.IsActive = true;
                KudosBusyIndicator.Content = message;
            }
            else
            {
                KudosBusyIndicator.IsActive = false;
                KudosBusyIndicator.Content = "";
            }
        }

        #endregion

        #region Static Helpers

        private static ObservableCollection<Kudos> LoadKudos()
        {
            return new ObservableCollection<Kudos>
            {
                new Kudos
                {
                    Title = "Play Ad", 
                    Category = KudoCategory.Free, 
                    Price = "Free", 
                    ImageUrl = "/Images/VideoAd_Colored.png"
                },
                new Kudos
                {
                    Title = "Store Rating", 
                    Category = KudoCategory.Free, 
                    Price = "Free", 
                    ImageUrl = "/Images/4starStar_Colored.png"
                },
                new Kudos
                {
                    Title = "Remove Ads",
                    Category = KudoCategory.Durable,
                    StoreId = StoreIds.CoverApiFeeKudoStoreId,
                    Price = "$0.99",
                    ImageUrl = "/Images/RemoveAdKudo_Colored.png"
                },
                new Kudos
                {
                    Title = "Champion",
                    Category = KudoCategory.Subscription,
                    StoreId = StoreIds.RecurringKudos1StoreId,
                    Price = "$1.09 (mth)",
                    ImageUrl = "/Images/RecurringKudo_Colored.png"
                },
                new Kudos
                {
                    Title = "Coffee",
                    Category = KudoCategory.Consumable,
                    StoreId = StoreIds.CoffeeKudoStoreId,
                    Price = "$2.49",
                    ImageUrl = "/Images/CoffeeKudo_Colored.png"
                },
                new Kudos
                {
                    Title = "1mo API Fee",
                    Category = KudoCategory.Consumable,
                    StoreId = StoreIds.CoverApiFeeKudoStoreId,
                    Price = "$3.99",
                    ImageUrl = "/Images/APIFeeKudo_Colored.png"
                },
                new Kudos
                {
                    Title = "Lunch",
                    Category = KudoCategory.Consumable,
                    StoreId = StoreIds.LunchKudoStoreId,
                    Price = "$4.89",
                    ImageUrl = "/Images/LunchKudo_Colored.png"
                },
                new Kudos
                {
                    Title = "Dinner",
                    Category = KudoCategory.Consumable,
                    StoreId = StoreIds.DinnerKudoStoreId,
                    Price = "$19.49",
                    ImageUrl = "/Images/DinnerKudo_Colored.png"
                }
            };
        }

        #endregion
    }
}
