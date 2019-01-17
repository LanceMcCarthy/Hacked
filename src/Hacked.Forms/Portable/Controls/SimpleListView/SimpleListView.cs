using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
    /// <summary>
    /// A bare minimum list control, based off RepeaterView, intended for use inside an ItemTemplate of a more complex list control like RadListView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class SimpleListView : StackLayout
    {
        private DataTemplateSelector _currentItemSelector;
        private IDisposable _collectionChangedHandle;

        public SimpleListView()
        {
            Spacing = 0;
        }

        #region Bindable Properties

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(SimpleListView),
            default(DataTemplate));
        
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<object>),
            typeof(SimpleListView),
            Enumerable.Empty<object>(),
            BindingMode.OneWay,
            null,
            OnItemsChanged);

        public static BindableProperty ItemClickCommandProperty = BindableProperty.Create(
            nameof(ItemClickCommand),
            typeof(ICommand),
            typeof(SimpleListView),
            default(ICommand));

        public static readonly BindableProperty TemplateSelectorProperty = BindableProperty.Create(
            nameof(TemplateSelector),
            typeof(TemplateSelector),
            typeof(SimpleListView),
            default(TemplateSelector));

        public static readonly BindableProperty ItemTemplateSelectorProperty = BindableProperty.Create(
            nameof(ItemTemplateSelector),
            typeof(DataTemplateSelector),
            typeof(SimpleListView),
            default(DataTemplateSelector),
            BindingMode.OneWay,
            null,
            OnDataTemplateSelectorChanged);
        

        public IEnumerable<object> ItemsSource
        {
            get => (IEnumerable<object>) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        
        public TemplateSelector TemplateSelector
        {
            get => (TemplateSelector) GetValue(TemplateSelectorProperty);
            set => SetValue(TemplateSelectorProperty, value);
        }
        
        public ICommand ItemClickCommand
        {
            get => (ICommand) this.GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }
        
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate) GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }
        
        private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SimpleListView control)
            {
                control._collectionChangedHandle?.Dispose();

                control._collectionChangedHandle = new CollectionChangedHandle<View, object>(
                    control.Children,
                    (IEnumerable<object>)newValue,
                    control.ViewFor,
                    (view, model, index) => control.NotifyItemAdded(view, model));
            }
        }

        private static void OnDataTemplateSelectorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SimpleListView control)
            {
                if (control.ItemTemplate != null && newValue != null)
                    throw new ArgumentException("You can only use ItemTemplate or ItemTemplateSelector, not both.", nameof(ItemTemplateSelector));

                control._currentItemSelector = newValue as DataTemplateSelector;
            }
        }

        #endregion

        public delegate void RepeaterViewItemAddedEventHandler(object sender, RepeaterViewItemAddedEventArgs args);

        public event RepeaterViewItemAddedEventHandler ItemCreated;

        protected virtual void NotifyItemAdded(View view, object model)
        {
            ItemCreated?.Invoke(this, new RepeaterViewItemAddedEventArgs(view, model));
        }
        
        protected virtual DataTemplate GetTemplateFor(Type type)
        {
            DataTemplate retTemplate = null;

            if (TemplateSelector != null) retTemplate = TemplateSelector.TemplateFor(type);

            return retTemplate ?? ItemTemplate;
        }
        
        protected virtual View ViewFor(object item)
        {
            View view = null;

            if (_currentItemSelector != null)
            {
                view = this.ViewFor(item, _currentItemSelector);
            }

            if (view == null)
            {
                var template = GetTemplateFor(item.GetType());
                var content = template.CreateContent();

                if (!(content is View) && !(content is ViewCell))
                    throw new InvalidVisualObjectException(content.GetType());

                view = (content is View view1) ? view1 : ((ViewCell) content).View;
            }

            view.BindingContext = item;
            view.GestureRecognizers.Add(new TapGestureRecognizer { Command = ItemClickCommand, CommandParameter = item });
            return view;
        }
    }
}