using System;

namespace Hacked.Maui.Models;

public class NavigationMenuItem
{
    public string Title { get; set; }
    public string IconSource { get; set; }
    public Type TargetType { get; set; }
}