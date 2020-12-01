using System;
using Hacked.Core.Models;

namespace Hacked.Core.Args
{
    public class BreachInfoArgs : EventArgs
    {
        public Breach Breach { get; set; }
    }
}
