using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Redaction
{
    /// <summary>
    /// Defines how to create redactions for a matched <see cref="RedactionMatchRule"/>.
    /// </summary>
    public class RedactionCreationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedactionCreationOptions"/> class.
        /// </summary>
        public RedactionCreationOptions()
        {
            this.Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the redaction reason to use for all redactions created
        /// by a rule. Later when the redaction is actually applied, the
        /// redaction reason will be printed as text in the center of the
        /// redaction box to explain why the content was redacted. Default is
        /// <see langword="null"/>.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the color of the <see cref="Reason"/> text to use for
        /// all redactions created by a rule. Value must be a CSS-style,
        /// 6-character RGB hex value, like <c>"#FF0000"</c>
        /// (red), or <see langword="null"/> for the remote server default,
        /// typically <c>"#FFFFFF"</c> (white). Default is <see langword="null"/>.
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// Gets or sets the fill color of the redaction box to use for all
        /// redactions created by a rule. Value must be a CSS-style,
        /// 6-character RGB hex value, like <c>"#FF0000"</c>
        /// (red), or <see langword="null"/> for the remote server default,
        /// typically <c>"#000000"</c> (black). Default is <see langword="null"/>.
        /// </summary>
        public string FillColor { get; set; }

        /// <summary>
        /// Gets or sets the border color of the redaction box to use for all
        /// redactions created by a rule. Value must be a CSS-style,
        /// 6-character RGB hex value, like <c>"#FF0000"</c>
        /// (red), or <see langword="null"/> for the remote server default,
        /// typically <c>"#000000"</c> (black). Default is <see
        /// langword="null"/>.
        /// </summary>
        public string BorderColor { get; set; }

        /// <summary>
        /// Gets or sets the pixel width of the redaction box border to use for
        /// all redactions created by a rule, or <see langword="null"/> for the
        /// remote server default, typically <c>1</c>. Default is <see
        /// langword="null"/>.
        /// </summary>
        public uint? BorderThickness { get; set; }

        /// <summary>
        /// Gets or sets an arbitrary key/value string metadata to attach to all
        /// redactions created by a rule. Useful if your application will do any
        /// parsing or modifying of the markup JSON output.
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
    }
}
