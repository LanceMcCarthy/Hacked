using System;
using System.Runtime.Serialization;
using CommonHelpers.Common;

namespace Hacked.DataLayer.Models
{
    [DataContract]
    public class Breach : BindableBase
    {
        #region ***** Added Properties *****

        private bool isSelected;
        private bool isNew;

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public bool IsNew
        {
            get => isNew;
            set => SetProperty(ref isNew, value);
        }

        [DataMember]
        public string Id
        {
            get => this.Title;
            set => Title = value;
        }

        #endregion

        #region **** Original Model ******

        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public string BreachDate { get; set; }
        [DataMember]
        public DateTime AddedDate { get; set; }
        [DataMember]
        public int PwnCount { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string[] DataClasses { get; set; }
        [DataMember]
        public bool IsVerified { get; set; }
        [DataMember]
        public bool IsSensitive { get; set; }
        [DataMember]
        public string LogoType { get; set; }

        #endregion

        #region methods
        
        public override bool Equals(object obj)
        {
            var otherBreach = obj as Breach;

            // Note - sometimes this method gets passed a string[], just use base.Equals instead 
            if (otherBreach == null)
                return base.Equals(obj);

            try
            {
                return this.Title.ToLowerInvariant().Equals(otherBreach?.Title?.ToLowerInvariant());
            }
            catch
            {
                return this.Title.ToLowerInvariant() == otherBreach?.Title?.ToLowerInvariant();
            }
        }

        public override int GetHashCode() => this.Title?.GetHashCode() ?? base.GetHashCode();

        #endregion
    }
}


/* 
******************EXAMPLE RESULT*********************
[
    {
        "Title": "000webhost",
        "Name": "000webhost",
        "Domain": "000webhost.com",
        "BreachDate": "2015-3-1",
        "AddedDate": "2015-10-26T23:35:45Z",
        "PwnCount": 13545468,
        "Description": "In approximately March 2015, the free web hosting provider <a href=\"http://www.troyhunt.com/2015/10/breaches-traders-plain-text-passwords.html\" target=\"_blank\">000webhost suffered a major data breach</a> that exposed over 13 million customer records. The data was sold and traded before 000webhost was alerted in October. The breach included names, email addresses and plain text passwords.",
        "DataClasses": [
            "Email addresses",
            "IP addresses",
            "Names",
            "Passwords"
        ],
        "IsVerified": true,
        "IsSensitive": false,
        "LogoType": "png"
    },
    {
        "Title": "Adobe",
        "Name": "Adobe",
        "Domain": "adobe.com",
        "BreachDate": "2013-10-4",
        "AddedDate": "2013-12-04T00:00:00Z",
        "PwnCount": 152445165,
        "Description": "The big one. In October 2013, 153 million Adobe accounts were breached with each containing an internal ID, username, email, <em>encrypted</em> password and a password hint in plain text. The password cryptography was poorly done and <a href=\"http://stricture-group.com/files/adobe-top100.txt\" target=\"_blank\">many were quickly resolved back to plain text</a>. The unencrypted hints also <a href=\"http://www.troyhunt.com/2013/11/adobe-credentials-and-serious.html\" target=\"_blank\">disclosed much about the passwords</a> adding further to the risk that hundreds of millions of Adobe customers already faced.",
        "DataClasses": [
            "Email addresses",
            "Password hints",
            "Passwords",
            "Usernames"
        ],
        "IsVerified": true,
        "IsSensitive": false,
        "LogoType": "svg"
    },
    {
        "Title": "Flashback",
        "Name": "Flashback",
        "Domain": "flashback.se",
        "BreachDate": "2015-2-11",
        "AddedDate": "2015-02-12T05:42:12Z",
        "PwnCount": 40256,
        "Description": "In February 2015, <a href=\"http://www.flashback.se/\" target=\"_blank\">the Swedish forum known as Flashback</a> had sensitive internal data on 40k members published via the tabloid newspaper <a href=\"http://www.aftonbladet.se/\" target=\"_blank\">Aftonbladet</a>. The data was <a href=\"http://swedishsurveyor.com/2015/02/11/the-inquisition/\">allegedly sold to them via Researchgruppen</a> (The Research Group) <a href=\"http://www.technologyreview.com/photoessay/533426/the-troll-hunters/\" target=\"_blank\">who have a history of exposing otherwise anonymous users</a>, primarily those who they believe participate in &quot;troll like&quot; behaviour. The compromised data includes social security numbers, home and email addresses.",
        "DataClasses": [
            "Addresses",
            "Email addresses",
            "Government issued IDs"
        ],
        "IsVerified": true,
        "IsSensitive": false,
        "LogoType": "svg"
    },
    {
        "Title": "Gawker",
        "Name": "Gawker",
        "Domain": "gawker.com",
        "BreachDate": "2010-12-11",
        "AddedDate": "2013-12-04T00:00:00Z",
        "PwnCount": 1247574,
        "Description": "In December 2010, Gawker was attacked by the hacker collective &quot;Gnosis&quot; in retaliation for what was reported to be a feud between Gawker and 4Chan. Information about Gawkers 1.3M users was published along with the data from Gawker's other web presences including Gizmodo and Lifehacker. Due to the prevalence of password reuse, many victims of the breach <a href=\"http://www.troyhunt.com/2011/01/why-your-apps-security-design-could.html\" target=\"_blank\">then had their Twitter accounts compromised to send Acai berry spam</a>.",
        "DataClasses": [
            "Email addresses",
            "Passwords",
            "Usernames"
        ],
        "IsVerified": true,
        "IsSensitive": false,
        "LogoType": "svg"
    },
    {
        "Title": "MPGH",
        "Name": "MPGH",
        "Domain": "mpgh.net",
        "BreachDate": "2015-10-22",
        "AddedDate": "2015-10-26T03:20:20Z",
        "PwnCount": 3122898,
        "Description": "In October 2015, the multiplayer game hacking website <a href=\"http://www.mpgh.net\">MPGH was hacked</a> and 3.1 million user accounts disclosed. The vBulletin forum breach contained usernames, email addresses, IP addresses and salted hashes of passwords.",
        "DataClasses": [
            "Email addresses",
            "IP addresses",
            "Passwords",
            "Usernames"
        ],
        "IsVerified": true,
        "IsSensitive": false,
        "LogoType": "png"
    },
    {
        "Title": "Stratfor",
        "Name": "Stratfor",
        "Domain": "stratfor.com",
        "BreachDate": "2011-12-24",
        "AddedDate": "2013-12-04T00:00:00Z",
        "PwnCount": 859777,
        "Description": "In December 2011, &quot;Anonymous&quot; <a href=\"http://www.troyhunt.com/2011/12/5-website-security-lessons-courtesy-of.html\" target=\"_blank\">attacked the global intelligence company known as &quot;Stratfor&quot;</a> and consequently disclosed a veritable treasure trove of data including hundreds of gigabytes of email and tens of thousands of credit card details which were promptly used by the attackers to make charitable donations (among other uses). The breach also included 860,000 user accounts complete with email address, time zone, some internal system data and MD5 hashed passwords with no salt.",
        "DataClasses": [
            "Addresses",
            "Credit cards",
            "Email addresses",
            "Names",
            "Passwords",
            "Phone numbers",
            "Usernames"
        ],
        "IsVerified": true,
        "IsSensitive": false,
        "LogoType": "svg"
    }
]
*/

