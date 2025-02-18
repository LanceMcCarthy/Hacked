using System;
using System.Collections.Generic;
using CommonHelpers.Common;
using Newtonsoft.Json;

namespace Hacked.Core.Models;

public class Breach : BindableBase
{
    // **** API members ******

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Title")]
    public string Title { get; set; }

    [JsonProperty("Domain")]
    public string Domain { get; set; }

    [JsonProperty("BreachDate")]
    public DateTime? BreachDate { get; set; }

    [JsonProperty("AddedDate")]
    public string AddedDate { get; set; }

    [JsonProperty("ModifiedDate")]
    public string ModifiedDate { get; set; }

    [JsonProperty("PwnCount")]
    public long PwnCount { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("DataClasses")]
    public List<string> DataClasses { get; set; }

    [JsonProperty("IsVerified")]
    public bool IsVerified { get; set; }

    [JsonProperty("IsFabricated")]
    public bool IsFabricated { get; set; }

    [JsonProperty("IsSensitive")]
    public bool IsSensitive { get; set; }

    [JsonProperty("IsRetired")]
    public bool IsRetired { get; set; }

    [JsonProperty("IsSpamList")]
    public bool IsSpamList { get; set; }

    [JsonProperty("LogoPath")]
    public Uri LogoPath { get; set; }

    // ***** App specific members *****

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

    public string Id => Title;

    public override bool Equals(object obj)
    {
        // Note - sometimes this method gets passed a string[], just use base.Equals instead 
        if (obj is not Breach otherBreach)
            return base.Equals(obj);

        try
        { 
            return Title.ToLowerInvariant().Equals(otherBreach.Title?.ToLowerInvariant());
        }
        catch
        {
            try
            {
                return Title.ToLowerInvariant() == otherBreach.Title?.ToLowerInvariant();
            }
            catch (Exception e)
            {
                return this.Equals(obj);
            }
            
        }
    }

    public override int GetHashCode() => Title?.GetHashCode() ?? base.GetHashCode();
}

/* 
******************EXAMPLE RESULT*********************
[
   {
   "Name":"Adobe",
   "Title":"Adobe",
   "Domain":"adobe.com",
   "BreachDate":"2013-10-04",
   "AddedDate":"2013-12-04T00:00Z",
   "ModifiedDate":"2013-12-04T00:00Z",
   "PwnCount":152445165,
   "Description":"In October 2013, 153 million Adobe accounts were breached with each containing an internal ID, username, email, <em>encrypted</em> password and a password hint in plain text. The password cryptography was poorly done and <a href=\"http://stricture-group.com/files/adobe-top100.txt\" target=\"_blank\" rel=\"noopener\">many were quickly resolved back to plain text</a>. The unencrypted hints also <a href=\"http://www.troyhunt.com/2013/11/adobe-credentials-and-serious.html\" target=\"_blank\" rel=\"noopener\">disclosed much about the passwords</a> adding further to the risk that hundreds of millions of Adobe customers already faced.",
   "DataClasses":["Email addresses","Password hints","Passwords","Usernames"],
   "IsVerified":True,
   "IsFabricated":False,
   "IsSensitive":False,
   "IsRetired":False,
   "IsSpamList":False,
   "LogoPath":"https://haveibeenpwned.com/Content/Images/PwnedLogos/Adobe.png"
   },
   {
   "Name":"BattlefieldHeroes",
   "Title":"Battlefield Heroes",
   "Domain":"battlefieldheroes.com",
   "BreachDate":"2011-06-26",
   "AddedDate":"2014-01-23T13:10Z",
   "ModifiedDate":"2014-01-23T13:10Z",
   "PwnCount":530270,
   "Description":"In June 2011 as part of a final breached data dump, the hacker collective &quot;LulzSec&quot; <a href=\"http://www.rockpapershotgun.com/2011/06/26/lulzsec-over-release-battlefield-heroes-data\" target=\"_blank\" rel=\"noopener\">obtained and released over half a million usernames and passwords from the game Battlefield Heroes</a>. The passwords were stored as MD5 hashes with no salt and many were easily converted back to their plain text versions.",
   "DataClasses":["Passwords","Usernames"],
   "IsVerified":True,
   ""IsFabricated":False,
   "IsSensitive":False,
   "IsRetired":False,
   "IsSpamList":False,
   "LogoPath":"https://haveibeenpwned.com/Content/Images/PwnedLogos/BattlefieldHeroes.png"
   }
   ]
*/
