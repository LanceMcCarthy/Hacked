using System;

namespace Hacked.Core.Args;

public class AdRequestedArgs : EventArgs
{
    public AdRequestedArgs(string placementId)
    {
        this.PlacementId = placementId;
    }

    public string PlacementId { get; set; }
}
