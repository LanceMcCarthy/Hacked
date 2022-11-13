namespace Hacked.Core.Common;

// ReSharper disable InconsistentNaming
public static class HibpConstants
{
    // HTTP Headers
    public const string HibpApiBaseAddress = "https://haveibeenpwned.com/api/v3/";
    public const string HibpUserAgentKey = "User-Agent";
    public const string HibpUserAgentValue = "Hacked-for-Windows-Universal";
    public const string HibpApiHeaderKey = "hibp-api-key";

    // Routes for Breaches API
    public const string ApiRoute_Breaches = "breaches";
    public const string ApiRoute_BreachedAccount = "breachedaccount";
    public const string ApiRoute_DataClasses = "dataclasses";

    // Routes for Password API
    public const string ApiRoute_Range = "range";
}