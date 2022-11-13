namespace Hacked.Core.Common;

/// <summary>
/// Test accounts exist to demonstrate different behaviors. All accounts are on the domain "hibp-integration-tests.com".
/// </summary>
public static class HibpTestAccounts
{
    /// <summary>
    /// Returns one breach and one paste.
    /// </summary>
    public const string account_exists = "account-exists@hibp-integration-tests.com";
    
    /// <summary>
    /// Returns three breaches.
    /// </summary>
    public const string multiple_breaches = "multiple-breaches@hibp-integration-tests.com";
    
    /// <summary>
    /// Returns one breach being "Adobe". An inactive breach also exists against this account in the underlying data structure.
    /// </summary>
    public const string not_active_and_active_breach = "not-active-and-active-breach@hibp-integration-tests.com";
    
    /// <summary>
    /// An inactive data breach also exists against this account in the underlying data structure.
    /// </summary>
    public const string not_active_breach = "not-active-breach@hibp-integration-tests.com";
    /// <summary>
    /// Returns no breaches and no pastes. This account is opted-out of both pastes and breaches in the underlying data structure.
    /// </summary>
    public const string opt_out = "opt-out@hibp-integration-tests.com";
    
    /// <summary>
    /// Returns no breaches and no pastes. This account is opted-out of breaches in the underlying data structure.
    /// </summary>
    public const string opt_out_breach = "opt-out-breach@hibp-integration-tests.com";
    
    /// <summary>
    /// Returns no breaches and one paste. A sensitive breach exists against this account in the underlying data structure.
    /// </summary>
    public const string paste_sensitive_breach = "paste-sensitive-breach@hibp-integration-tests.com";
    
    /// <summary>
    /// Returns no breaches and no pastes. This account is permanently opted-out of both breaches and pastes in the underlying data structure.
    /// </summary>
    public const string permanent_opt_out = "permanent-opt-out@hibp-integration-tests.com";
    
    /// <summary>
    /// Returns two non-sensitive breaches and no pastes. A sensitive breach exists against this account in the underlying data structure.
    /// </summary>
    public const string sensitive_and_other_breaches = "sensitive-and-other-breaches@hibp-integration-tests.com";
    
    /// <summary>
    /// Returns no breaches and no pastes. A sensitive breach exists against this account in the underlying data structure.
    /// </summary>
    public const string sensitive_breach = "sensitive-breach@hibp-integration-tests.com";
    
    /// <summary>
    /// Returns one unverified breach and no pastes.
    /// </summary>
    public const string unverified_breach = "unverified-breach@hibp-integration-tests.com";
}