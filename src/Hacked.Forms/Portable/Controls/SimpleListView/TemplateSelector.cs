using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
	[ContentProperty("Templates")]
    public class TemplateSelector : BindableObject
    {
        public TemplateSelector()
        {
            Templates = new DataTemplateCollection();
        }

        #region Bindable Properties

        public static BindableProperty TemplatesProperty = BindableProperty.Create(
            nameof(Templates),
            typeof(DataTemplateCollection),
            typeof(TemplateSelector),
            default(DataTemplateCollection),
            BindingMode.OneWay,
            null,
            OnTemplatesChanged);

        public static BindableProperty SelectorFunctionProperty = BindableProperty.Create(
            nameof(ExceptionOnNoMatch),
            typeof(Func<Type, DataTemplate>),
            typeof(TemplateSelector),
            default(Func<Type, DataTemplate>));

        public static BindableProperty ExceptionOnNoMatchProperty = BindableProperty.Create(
            nameof(SelectorFunction),
            typeof(bool),
            typeof(TemplateSelector),
            true);
        
        public bool ExceptionOnNoMatch
        {
            get => (bool) GetValue(ExceptionOnNoMatchProperty);
            set => SetValue(ExceptionOnNoMatchProperty, value);
        }

        public DataTemplateCollection Templates
        {
            get => (DataTemplateCollection) GetValue(TemplatesProperty);
            set => SetValue(TemplatesProperty, value);
        }

        public Func<Type, DataTemplate> SelectorFunction
        {
            get => (Func<Type, DataTemplate>) GetValue(SelectorFunctionProperty);
            set => SetValue(SelectorFunctionProperty, value);
        }

        private static void OnTemplatesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TemplateSelector selector)
            {
                if (oldValue != null)
                    ((DataTemplateCollection)oldValue).CollectionChanged -= selector.TemplateSetChanged;

                ((DataTemplateCollection)newValue).CollectionChanged += selector.TemplateSetChanged;

                selector.DataTemplateCache = null;
            }
        }

        #endregion
        
        private void TemplateSetChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DataTemplateCache = null;
        }

        private Dictionary<Type, DataTemplate> DataTemplateCache { get; set; }

        public DataTemplate TemplateFor(Type type)
        {
            var typesExamined = new List<Type>();
            var template = TemplateForImpl(type, typesExamined);
            if (template == null && ExceptionOnNoMatch)
                throw new NoDataTemplateMatchException(type, typesExamined);
            return template;
        }

        private DataTemplate TemplateForImpl(Type type, List<Type> examined)
        {
            if (type == null)
            {
                return null;
            }

            examined.Add(type);
            
            DataTemplateCache = DataTemplateCache ?? new Dictionary<Type, DataTemplate>();

            DataTemplate retTemplate;
            
            if (SelectorFunction != null)
            {
                retTemplate = SelectorFunction(type);
            }
            else
            {
                if (DataTemplateCache.ContainsKey(type))
                {
                    return DataTemplateCache[type];
                }

                retTemplate = Templates.Where(x => x.Type == type).Select(x => x.WrappedTemplate).FirstOrDefault();

                retTemplate = retTemplate ?? type.GetTypeInfo().ImplementedInterfaces.Select(x => TemplateForImpl(x, examined)).FirstOrDefault();
                retTemplate = retTemplate ?? TemplateForImpl(type.GetTypeInfo().BaseType, examined);
                retTemplate = retTemplate ?? Templates.Where(x => x.IsDefault).Select(x => x.WrappedTemplate).FirstOrDefault();
            }
            
            DataTemplateCache[type] = retTemplate;

            return retTemplate;
        }

        public View ViewFor(object item)
        {
            var template = TemplateFor(item.GetType());

            var content = template.CreateContent();

            if (!(content is View) && !(content is ViewCell))
            {
                throw new InvalidVisualObjectException(content.GetType());
            }

            var view = (content is View view1) ? view1 : ((ViewCell) content).View;

            view.BindingContext = item;

            return view;
        }
    }
}
