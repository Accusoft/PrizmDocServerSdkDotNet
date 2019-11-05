namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// Defines a line of text for a header or footer.
    /// The line may contain text placed on the left, placed in the center, and/or placed on the right.
    /// </summary>
    public class HeaderFooterLine
    {
        /// <summary>
        /// Gets or sets text to place on the left side of the line.
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// Gets or sets text to place in the center of the line.
        /// </summary>
        public string Center { get; set; }

        /// <summary>
        /// Gets or sets text to place on the right side of the line.
        /// </summary>
        public string Right { get; set; }
    }
}
