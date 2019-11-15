using System;
using System.Text.RegularExpressions;

namespace Accusoft.PrizmDocServer.Redaction
{
    /// <summary>
    /// Defines a rule to create redactions for all parts of the document where the text matches a given regular expression <see cref="Pattern"/>.
    /// </summary>
    public class RegexRedactionMatchRule : RedactionMatchRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexRedactionMatchRule"/> class.
        /// </summary>
        public RegexRedactionMatchRule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexRedactionMatchRule"/> class.
        /// </summary>
        /// <param name="pattern">Regular expression pattern identifying document text for which redactions should be created.</param>
        public RegexRedactionMatchRule(string pattern)
        {
            this.Pattern = pattern;
        }

        /// <summary>
        /// Gets or sets the regular expression identifying document text for which redactions should be created.
        /// </summary>
        public string Pattern { get; set; }
    }
}
