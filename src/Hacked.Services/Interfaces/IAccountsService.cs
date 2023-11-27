using Hacked.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hacked.Services.Interfaces;

public interface IAccountsService
{
    ObservableCollection<MonitoredAccount> CurrentAccounts { get; set; }

    Task SaveAccountsAsync();

    Task LoadAccountsAsync();

    Task<Tuple<bool, string>> ImportBackupAsync();

    Task<Tuple<bool, string>> ExportBackupAsync();
}
