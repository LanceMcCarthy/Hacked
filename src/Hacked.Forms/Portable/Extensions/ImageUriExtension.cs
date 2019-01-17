using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hacked.Forms.Portable.Extensions
{
    [ContentProperty("Source")]
    public class ImageUriExtension : IMarkupExtension
    {
        public ImageUriExtension()
        {
            
        }

        public string ImageUri { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetImageUri().Result;
        }

        private async Task<ImageSource> GetImageUri()
        {
            try
            {
                if (string.IsNullOrEmpty(ImageUri))
                    return null;
                
                if (File.Exists(ImageUri))
                {
                    return new FileImageSource { File = ImageUri };
                }
                else
                {
                    return new UriImageSource { Uri = new Uri(ImageUri) };
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"------ImageUriExtension Exception--------\r\n{ex}");
                return null;
            }
        }
    }
}
