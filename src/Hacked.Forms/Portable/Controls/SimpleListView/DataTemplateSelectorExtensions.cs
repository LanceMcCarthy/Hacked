using System;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
    public static class DataTemplateSelectorExtensions
    {
        public static Cell CellFor(this BindableObject bindable, object item, DataTemplateSelector selector)
        {
            var template = selector?.SelectTemplate(item, bindable);

            if (template == null)
                return null;

            var templateInstance = template.CreateContent();

            if (templateInstance is Cell cell)
            {
                return cell;
            }

            if (templateInstance is View view)
            {
                return new ViewCell { View = view };
            }

            throw new InvalidOperationException("DataTemplate must be either a Cell or a View");
        }
        
        public static View ViewFor(this BindableObject bindable, object item, DataTemplateSelector selector)
        {
            var template = selector?.SelectTemplate(item, bindable);

            if (template == null)
                return null;

            var templateInstance = template.CreateContent();

            if (templateInstance is View view)
            {
                return view;
            }

            throw new InvalidOperationException("DataTemplate must be a View");
        }
    }
}