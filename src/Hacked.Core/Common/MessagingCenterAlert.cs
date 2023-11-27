using Hacked.Core.Interfaces;
using System;

namespace Hacked.Core.Common;

public class MessagingCenterAlert : IMessagingCenterItem
{
    public string Title { get; set; }
        
    public string Message { get; set; }
        
    public string Cancel { get; set; }
        
    public Action OnCompleted { get; set; }
}