using System;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
    public interface IDataTemplateWrapper
    {
        bool IsDefault { get; set; }

        DataTemplate WrappedTemplate { get; set; }

        Type Type { get; }
    }
}