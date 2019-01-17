using System;
using System.Runtime.CompilerServices;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
    public class InvalidVisualObjectException : Exception
    {
        private InvalidVisualObjectException() { }

        public InvalidVisualObjectException(Type inflatedType, [CallerMemberName] string name = null) :
            base($"Invalid ItemTemplate content in {name}. DataTemplates must be a Xamarin.Forms.View or Xamarin.Forms.ViewCell.\nActual Type received: [{inflatedType.Name}]")
        { }

        public Type InflatedType { get; set; }

        public string MemberName { get; set; }
    }
}