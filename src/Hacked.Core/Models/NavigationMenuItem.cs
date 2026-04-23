using System;

namespace Hacked.Core.Models;

public class NavigationMenuItem
{
    public string Title { get; set; } = string.Empty;
    public string IconSource { get; set; } = string.Empty;
    public Type TargetType { get; set; } = typeof(object);
}