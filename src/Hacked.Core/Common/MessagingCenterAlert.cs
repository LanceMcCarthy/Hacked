using Hacked.Core.Interfaces;
using System;

namespace Hacked.Core.Common;

public class MessagingCenterAlert : IMessagingCenterItem
{
    public string Title { get; set; } = "Title";

    public string Message { get; set; } = "Message";

    public string Cancel { get; set; } = "Cancel";
        
    public Action OnCompleted { get; set; }
}