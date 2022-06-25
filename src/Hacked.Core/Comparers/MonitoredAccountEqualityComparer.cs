using System.Collections.Generic;
using Hacked.Core.Models;

namespace Hacked.Core.Comparers;

public class MonitoredAccountEqualityComparer : IEqualityComparer<MonitoredAccount>
{
    public bool Equals(MonitoredAccount a, MonitoredAccount b)
    {
        return a?.Address == b?.Address &&
               b?.Address == a?.Address;
    }

    public int GetHashCode(MonitoredAccount account)
    {
        unchecked
        {
            int hashCode = account.Address.GetHashCode();
            hashCode = (hashCode * 397) ^ account.Address.GetHashCode();
            return hashCode;
        }
    }
}
