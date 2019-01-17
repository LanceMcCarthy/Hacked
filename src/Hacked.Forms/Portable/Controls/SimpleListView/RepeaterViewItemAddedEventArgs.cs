using System;
using Xamarin.Forms;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
    public class RepeaterViewItemAddedEventArgs : EventArgs
    {
        public RepeaterViewItemAddedEventArgs(View view, object model)
        {
            View = view;
            Model = model;
        }
        
        public View View { get; set; }
        
        public object Model { get; set; }
    }
}