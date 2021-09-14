using Microsoft.Maui.Controls;
using System;

namespace Hacked.Maui.Helpers
{
    public static class TaskHelpers
    {
        public static bool RunOnUiThread(Action a)
        {
            try
            {
                Device.BeginInvokeOnMainThread(a);
                return true;
            }
            catch (Exception ex)
            {
                //TODO log exception
                Debug.WriteLine($"RunOnUiThread Helper Exception: {ex.Message}");
                return false;
            }
        }
    }
}
