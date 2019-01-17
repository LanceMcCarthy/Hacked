using System;
using System.Collections.Generic;
using System.Linq;

namespace Hacked.Forms.Portable.Controls.SimpleListView
{
	public class NoDataTemplateMatchException : Exception
    {
        private NoDataTemplateMatchException() { }

        public NoDataTemplateMatchException(Type toMatch, List<Type> candidates) :
            base($"Could not find a template for type [{toMatch.Name}]")
        {
            AttemptedMatch = toMatch;
            TypesExamined = candidates;
            TypeNamesExamined = TypesExamined.Select(x => x.Name).ToList();
        }

        public Type AttemptedMatch { get; set; }

        public List<Type> TypesExamined { get; set; }

        public List<string> TypeNamesExamined { get; set; }
    }
}