using System;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
    [ContentProperty("WrappedTemplate")]
    public class DataTemplateWrapper<T> : BindableObject, IDataTemplateWrapper
    {
        public static readonly BindableProperty WrappedTemplateProperty = BindableProperty.Create(
            nameof(WrappedTemplate),
            typeof(DataTemplate),
            typeof(DataTemplateWrapper<T>),
            default(DataTemplate));

        public static readonly BindableProperty IsDefaultProperty = BindableProperty.Create(
            nameof(WrappedTemplate),
            typeof(bool),
            typeof(DataTemplateWrapper<T>),
            false);
        
        public bool IsDefault
        {
            get => (bool)GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        public DataTemplate WrappedTemplate
        {
            get => (DataTemplate)GetValue(WrappedTemplateProperty);
            set => SetValue(WrappedTemplateProperty, value);
        }

        public Type Type => typeof(T);
    }
}