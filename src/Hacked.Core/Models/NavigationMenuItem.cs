using System;

namespace Hacked.Core.Models;

public class NavigationMenuItem
{
    public string Title { get; set; }
    public string IconSource { get; set; }
    public Type TargetType { get; set; }
}