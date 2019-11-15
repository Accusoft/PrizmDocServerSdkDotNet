namespace Accusoft.PrizmDocServer.Redaction
{
    /// <summary>
    /// Base class for all redaction match rule types.
    /// </summary>
    public abstract class RedactionMatchRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedactionMatchRule"/> class.
        /// </summary>
        protected RedactionMatchRule()
        {
            this.RedactWith = new RedactionCreationOptions();
        }

        /// <summary>
        /// Gets or sets the <see cref="RedactionCreationOptions"/> to use for all redactions created from this rule.
        /// </summary>
        public RedactionCreationOptions RedactWith { get; set; }
    }
}
