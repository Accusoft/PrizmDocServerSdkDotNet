namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// PNG-specific conversion output options.
    /// </summary>
    public class PngDestinationOptions
    {
        /// <summary>
        /// Gets or sets the maximum pixel width of the output PNG(s), expressed
        /// as a CSS-style string, e.g. "800px". When specified, the output
        /// image is guaranteed to never be wider than the specified value and
        /// its aspect ratio will be preserved. This is useful if you need all
        /// of your output images to fit within a single column.
        /// </summary>
        public string MaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the maximum pixel height of the output PNG(s),
        /// expressed as a CSS-style string, e.g. "600px". When specified, the
        /// output image is guaranteed to never be taller than the specified
        /// value and its aspect ratio will be preserved. This is useful if you
        /// need all of your output images to fit within a single row.
        /// </summary>
        public string MaxHeight { get; set; }
    }
}
