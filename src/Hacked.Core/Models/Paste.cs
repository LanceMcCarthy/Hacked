using System;

namespace Hacked.Core.Models
{
    public class Paste
    {
        public string Source { get; set; }

        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public int EmailCount { get; set; }
    }
}

/*
 * 
Source - The paste service the record was retrieved from. Current values are: Pastebin, Pastie, Slexy, Ghostbin, QuickLeak, JustPaste, AdHocUrl, OptOut
Id - The ID of the paste as it was given at the source service. Combined with the "Source" attribute, this can be used to resolve the URL of the paste.
Title -	The title of the paste as observed on the source site. This may be null and if so will be omitted from the response.
Date -	The date and time (precision to the second) that the paste was posted. This is taken directly from the paste site.
EmailCount - The number of emails that were found when processing the paste. Emails are extracted by using the regular expression \b+[a-zA-Z0-9\.\-_\+]+@[a-zA-Z0-9\.\-]+\.[a-zA-Z]+\b
 * 
 */
