namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// OCR-specific conversion options.
    /// </summary>
    public class OcrOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OcrOptions"/> class.
        /// </summary>
        public OcrOptions()
        {
            this.Language = "english";
        }

        /// <summary>
        /// Gets or sets the OCR language. Default is <c>"english"</c>.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the default DPI to use whenever an input document does
        /// not itself specify any DPI information.
        /// </summary>
        public DpiOptions DefaultDpi { get; set; }
    }
}
