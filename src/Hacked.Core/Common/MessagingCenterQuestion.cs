using System;
using Hacked.Core.Interfaces;

namespace Hacked.Core.Common;

public class MessagingCenterQuestion : IMessagingCenterItem
{
    public string Title { get; set; }
        
    public string Message { get; set; }
        
    public string Okay { get; set; }
        
    public string Cancel { get; set; }
        
    public Action OnOkay { get; set; }

    public Action OnCancel { get; set; }
}