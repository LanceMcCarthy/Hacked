using Hacked.Core.Args;
using System;
using System.Diagnostics;
using VungleSDK;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Hacked;

public sealed partial class MainPage
{
    private async void PlayAdButton_OnClick(object sender, RoutedEventArgs e)
    {
        await VungleAd1.PlayAdAsync();
    }

    // A better option might be to make this Ad AutoCached
    private void VungleSdk_OnInitCompleted(object sender, ConfigEventArgs e)
    {
        try
        {
            vungleSdk.LoadAd(VungleMainInterstitialPlacementId);
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex.ToString());
        }
    }

    private async void VungleSdkOnAdPlayableChanged(object sender, AdPlayableEventArgs e)
    {
        Trace.WriteLine($"Ad Changed - Placement: {e.Placement}, IsPlayable: {e.AdPlayable}");

        if (VungleMainInterstitialPlacementId.Equals(e.Placement))
        {
            if (e.AdPlayable)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        var playable = vungleSdk.IsAdPlayable(e.Placement);

                        PlayAdButton.IsEnabled = playable;

                        if (!playable)
                        {
                            // Possible "sleep" code, try to Load Ad Again
                            vungleSdk.LoadAd(e.Placement);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                });
            }
            else
            {
                vungleSdk.LoadAd(VungleMainInterstitialPlacementId);

                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PlayAdButton.IsEnabled = false;
                });
            }
        }

        if (VungleKudoPlacementId.Equals(e.Placement))
        {
            if (e.AdPlayable)
            {
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        var playable = vungleSdk.IsAdPlayable(e.Placement);

                        // TODO work on disabling replay-ability too soon
                        //if (KudosCtrl.Kudoses.FirstOrDefault() is Kudos adKudo)
                        //{
                        //    adKudo.IsBusy = playable;
                        //}

                        if (!playable)
                        {
                            vungleSdk.LoadAd(e.Placement);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                });
            }
            else
            {
                vungleSdk.LoadAd(VungleKudoPlacementId);

                // TODO work on disabling replay-ability too soon
                //await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                //    if (KudosCtrl.Kudoses.FirstOrDefault() is Kudos adKudo)
                //    {
                //        adKudo.IsBusy = true;
                //    }
                //});
            }
        }
    }

    private async void VungleAd1_Start(object sender, AdEventArgs e)
    {
        await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            VungleAd1.IsHitTestVisible = true;
        });
    }

    private async void VungleAd1_End(object sender, AdEndEventArgs e)
    {
        await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            VungleAd1.IsHitTestVisible = false;
        });
    }

    private void VungleSdk_Diagnostic(object sender, DiagnosticLogEvent e)
    {
        if (e.Message != null && (e.Message.ToLower().Contains("exception") || e.Message.ToLower().Contains("error")))
        {
            Trace.WriteLine($"VungleAd1: {e.Message}");
        }
    }

    private async void KudoAdRequested(object sender, AdRequestedArgs e)
    {
        await vungleSdk.PlayAdAsync(new AdConfig { Placement = e.PlacementId }, e.PlacementId);
    }
}
