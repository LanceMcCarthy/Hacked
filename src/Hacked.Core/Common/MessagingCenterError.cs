using System;
using Hacked.Core.Interfaces;

namespace Hacked.Core.Common;

public class MessagingCenterError : IMessagingCenterItem
{
    public string Caller { get; set; }
        
    public Exception Exception { get; set; }
}