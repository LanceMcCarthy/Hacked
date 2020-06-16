using Hacked.Core.Models;
using Hacked.Forms.Portable.ViewModels;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Views
{
    public partial class BreachDetailsPage : ContentPage
    {
        public BreachDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!(BindingContext is Breach breach))
                return;
            
            //full CSS on website
            //var css = @"<link rel=""stylesheet"" href=""https://haveibeenpwned.com/content/css/pwned?v=VsgPHjoBsI9jFFQfQL0hWSa6NVxeEMtRHqh94DzOVWM1"" >";

            //short CSS on dropbox
            //var css = @"<link rel=""stylesheet"" href=""https://dl.dropboxusercontent.com/u/47517502/AppResourcesCache/Hacked/hackedLogoStyle.css"" >";

            //html with inline css (WORKS BEST)
            //var html = $@"<html>
            //                  <head>
            //                     <style>
            //                        body {{
            //                              background-color: #191919;
            //                              color: #d3d3d3;
            //                         }}
            //                         div {{
            //                              display: block;
            //                         }}
            //                         .text-center {{
            //                              text-align: center;
            //                         }}
            //                         .container {{
            //                              padding-right: 10px;
            //                              padding-left: 10px;
            //                              margin-right: auto;
            //                              margin-left: auto;
            //                          }}
            //                          .pwnLogo.tiny {{
            //                              height: 28px;
            //                              max-width: 80px;
            //                              width: auto;
            //                           }}
            //                           img {{
            //                              vertical-align: middle;
            //                              border: 0;
            //                           }}
            //                      </style>
            //                    </head>
            //                 <body>
            //                    <div class=""container text-center"">
            //                      <img class=""pwnLogo tiny"" src=""https://az594751.vo.msecnd.net/cdn/{breach.Name}.{breach.LogoType}"" alt=""load failed""/>
            //                    </div>
            //                 </body>
            //              </html>";
            

            ////load html using HtmlWebViewSource
            //LogoWebView.Source = new HtmlWebViewSource { Html = html };

            if (!breach.IsNew)
            {
                breach.IsNew = false;
                ViewModelLocator.Main.SaveAccounts();
            }
        }
    }
}
