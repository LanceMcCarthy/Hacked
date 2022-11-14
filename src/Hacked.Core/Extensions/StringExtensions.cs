using System.Security.Cryptography;
using System.Text;

namespace Hacked.Core.Extensions;

public static class StringExtensions
{
    public static string Hash(this string password)
    {
        using var sha1 = new SHA1Managed();

        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));

        var sb = new StringBuilder(hash.Length * 2);

        foreach (var b in hash)
        {
            // can be "x2" if you want lowercase
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
}