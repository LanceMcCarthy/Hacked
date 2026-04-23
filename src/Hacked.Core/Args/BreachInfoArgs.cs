using Hacked.Core.Models;
using System;

namespace Hacked.Core.Args;

public class BreachInfoArgs : EventArgs
{
    public Breach Breach { get; set; } = new();
}
