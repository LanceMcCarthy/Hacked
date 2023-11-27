using Hacked.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hacked.Services.Interfaces;

public interface IPwndBreachService
{
    Task<ObservableCollection<Breach>> CheckForBreachesAsync(MonitoredAccount account, bool truncateResponse = false);

    Task<ObservableCollection<Breach>> GetAllKnownBreachesAsync(bool truncateResponse = false);

    Task<List<string>> GetAllKnownDataClassesAsync();

    Task<ObservableCollection<Breach>> GetPastesAsync(string emailAddress, bool truncateResponse = false);
}