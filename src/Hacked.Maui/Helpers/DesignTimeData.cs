using Hacked.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hacked.Maui.Helpers
{
    public class DesignTimeData
    {
        public static ObservableCollection<MonitoredAccount> GenerateSampleAccounts() => new ObservableCollection<MonitoredAccount>
        {
            new MonitoredAccount
            {
                Address = "foo@bar.com",
                LastUpdated = DateTime.Now,
                Breaches = new ObservableCollection<Breach>
                {
                    new Breach
                    {
                        IsNew = true,
                        Title = "000webhost",
                        Name = "000webhost",
                        Domain = "000webhost.com",
                        BreachDate = new DateTime(2013,10,4),
                        ModifiedDate = DateTime.Now.ToString("d"),
                        AddedDate = DateTime.Now.ToString("d"),
                        PwnCount = 13545468,
                        Description = "In approximately March 2015, the free web hosting provider <a href=\"http://www.troyhunt.com/2015/10/breaches-traders-plain-text-passwords.html\" target=\"_blank\">000webhost suffered a major data breach</a> that exposed over 13 million customer records. The data was sold and traded before 000webhost was alerted in October. The breach included names, email addresses and plain text passwords.",
                        DataClasses = new List<string>() {"Email addresses", "IP addresses", "Names", "Passwords", "IP addresses", "Names", "Passwords", "IP addresses", "Names", "Passwords"},
                        IsVerified = true,
                        IsSensitive = false,
                        LogoPath = new Uri("https://haveibeenpwned.com/Content/Images/PwnedLogos/000webhost.png"),
                        IsSelected = true
                    },
                    new Breach
                    {
                        Title = "Adobe",
                        Name = "Adobe",
                        Domain = "adobe.com",
                        BreachDate = new DateTime(2013,10,4),
                        ModifiedDate = DateTime.Now.ToString("d"),
                        AddedDate = DateTime.Now.ToString("d"),
                        PwnCount = 152445165,
                        Description =
                            "The big one. In October 2013, 153 million Adobe accounts were breached with each containing an internal ID, username, email, <em>encrypted</em> password and a password hint in plain text. The password cryptography was poorly done and <a href=\"http://stricture-group.com/files/adobe-top100.txt\" target=\"_blank\">many were quickly resolved back to plain text</a>. The unencrypted hints also <a href=\"http://www.troyhunt.com/2013/11/adobe-credentials-and-serious.html\" target=\"_blank\">disclosed much about the passwords</a> adding further to the risk that hundreds of millions of Adobe customers already faced.",
                        DataClasses = new List<string>() {"Email addresses", "Password hints", "Passwords", "Usernames"},
                        IsVerified = true,
                        IsSensitive = false,
                        LogoPath = new Uri("https://haveibeenpwned.com/Content/Images/PwnedLogos/Adobe.png"),
                    },
                    new Breach
                    {
                        Title = "Flashback",
                        Name = "Flashback",
                        Domain = "flashback.se",
                        BreachDate = new DateTime(2013,10,4),
                        ModifiedDate = DateTime.Now.ToString("d"),
                        AddedDate = DateTime.Now.ToString("d"),
                        PwnCount = 40256,
                        Description =
                            "In February 2015, <a href=\"http://www.flashback.se/\" target=\"_blank\">the Swedish forum known as Flashback</a> had sensitive internal data on 40k members published via the tabloid newspaper <a href=\"http://www.aftonbladet.se/\" target=\"_blank\">Aftonbladet</a>. The data was <a href=\"http://swedishsurveyor.com/2015/02/11/the-inquisition/\">allegedly sold to them via Researchgruppen</a> (The Research Group) <a href=\"http://www.technologyreview.com/photoessay/533426/the-troll-hunters/\" target=\"_blank\">who have a history of exposing otherwise anonymous users</a>, primarily those who they believe participate in &quot;troll like&quot; behaviour. The compromised data includes social security numbers, home and email addresses.",
                        DataClasses = new List<string>() {"Addresses", "Email addresses", "Government issued IDs"},
                        IsVerified = true,
                        IsSensitive = false,
                        LogoPath = new Uri("https://haveibeenpwned.com/Content/Images/PwnedLogos/Flashback.png"),
                    }
                }
            },
            new MonitoredAccount
            {
                Address = "firstname.lastname@domain.com",
                LastUpdated = DateTime.Now,
                Breaches = new ObservableCollection<Breach>
                {
                    new Breach
                    {
                        Title = "000webhost",
                        Name = "000webhost",
                        Domain = "000webhost.com",
                        BreachDate = new DateTime(2013,10,4),
                        ModifiedDate = DateTime.Now.ToString("d"),
                        AddedDate = DateTime.Now.ToString("d"),
                        PwnCount = 13545468,
                        Description =
                            "In approximately March 2015, the free web hosting provider <a href=\"http://www.troyhunt.com/2015/10/breaches-traders-plain-text-passwords.html\" target=\"_blank\">000webhost suffered a major data breach</a> that exposed over 13 million customer records. The data was sold and traded before 000webhost was alerted in October. The breach included names, email addresses and plain text passwords.",
                        DataClasses = new List<string>() {"Email addresses", "IP addresses", "Names", "Passwords"},
                        IsVerified = true,
                        IsSensitive = false,
                        LogoPath = new Uri("https://haveibeenpwned.com/Content/Images/PwnedLogos/000webhost.png"),
                    }
                }
            }
        };
    }
}
