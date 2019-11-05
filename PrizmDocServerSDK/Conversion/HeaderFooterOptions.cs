using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// Defines header or footer content to be applied.
    /// </summary>
    public class HeaderFooterOptions
    {
        /// <summary>
        /// Gets or sets the collection of header or footer lines which should be applied to the output document.
        /// Each line may define text in the left, center, and/or right position.
        /// </summary>
        public List<HeaderFooterLine> Lines { get; set; }

        /// <summary>
        /// Gets or sets font family to use for the header or footer. The font family must be available on the remote server.
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the font size, in points, to use for the header or footer. Value must be a string with a number followed by <c>"pt"</c>, such as <c>"12.pt"</c>.
        /// </summary>
        public string FontSize { get; set; }

        /// <summary>
        /// Gets or sets the color to use for the header or footer. Value must be a CSS hex-based color value (like <c>"#FF0000"</c>).
        /// </summary>
        public string Color { get; set; }
    }
}
