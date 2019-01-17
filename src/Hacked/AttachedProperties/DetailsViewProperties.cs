using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hacked.AttachedProperties
{
    public class DetailsViewProperties
    {
        public static readonly DependencyProperty HtmlStringProperty =
           DependencyProperty.RegisterAttached("HtmlString", typeof(string), typeof(DetailsViewProperties), new PropertyMetadata("", OnHtmlStringChanged));
        
        public static string GetHtmlString(DependencyObject obj) { return (string)obj.GetValue(HtmlStringProperty); }
        public static void SetHtmlString(DependencyObject obj, string value) { obj.SetValue(HtmlStringProperty, value); }
        
        private static void OnHtmlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView wv = d as WebView;
            wv?.NavigateToString((string)e.NewValue);
        }
    }
}
