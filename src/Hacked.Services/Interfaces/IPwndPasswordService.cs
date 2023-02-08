using System.Threading.Tasks;

namespace Hacked.Services.Interfaces;

public interface IPwndPasswordService
{
    Task<string> CheckPasswordAsync(string password);
}