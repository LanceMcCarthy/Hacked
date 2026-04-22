using CommonHelpers.Messaging;
using CommunityToolkit.Mvvm.Messaging;
using Hacked.Maui.Views;

namespace Hacked.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("MonitoredAccounts/AccountDetails", typeof(AccountDetailsPage));
        Routing.RegisterRoute("MonitoredAccounts/AccountDetails/BreachDetails", typeof(BreachDetailsPage));

        WeakReferenceMessenger.Default.Register<MessagingCenterAlert>(this, HandleMessage);
        WeakReferenceMessenger.Default.Register<MessagingCenterQuestion>(this, HandleMessage);
        WeakReferenceMessenger.Default.Register<MessagingCenterError>(this, HandleMessage);
    }

    private async void HandleMessage(object r, IMessagingCenterItem m)
    {
        try
        {
            switch (m)
            {
                case MessagingCenterAlert msa:
                    await this.DisplayAlertAsync(msa.Title, msa.Message, msa.Cancel);
                    msa.OnCompleted?.Invoke();
                    break;
                case MessagingCenterQuestion msq:
                {
                    var result = await this.DisplayAlertAsync(msq.Title, msq.Message, msq.Okay, msq.Cancel);

                    if (result)
                    {
                        msq.OnOkay?.Invoke();
                    }
                    else
                    {
                        msq.OnCancel?.Invoke();
                    }

                    break;
                }
                case MessagingCenterError error:
                {
                    var message = "An unexpected error has occurred. If this happens again, contact us at awesome.apps@outlook.com and share the error message below" +
                                  $"\r\n\n{error.Caller} Error:" +
                                  $"\r\n {error.Exception.Message}";

                    await this.DisplayAlertAsync("Unexpected Error", message, "close");

                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}