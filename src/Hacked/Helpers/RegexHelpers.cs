using System.Text.RegularExpressions;

namespace Hacked.Helpers;

public static class RegexHelpers
{
    public static bool ValidateEmail(string emailAddress)
    {
        const string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
        var matches = Regex.Match(emailAddress, regexPattern);
        return matches.Success;
    }

    public static bool ValidatePhoneNumber(string phoneNumberString)
    {
        const string regexPattern = @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$";
        var matches = Regex.Match(phoneNumberString, regexPattern);
        return matches.Success;
    }
}
