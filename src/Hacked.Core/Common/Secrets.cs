using System;

namespace Hacked.Core.Common;

public static class Secrets
{
    // API Keys
    public static string HibpApiKey = Environment.GetEnvironmentVariable("HIBP_API_KEY") ?? "eed3414c0e504b7b3a3a9fd63a1ce26e";
}
