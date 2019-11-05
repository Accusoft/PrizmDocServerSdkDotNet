namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// TIFF-specific conversion output options.
    /// </summary>
    public class TiffDestinationOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the output should be
        /// multiple single-page TIFFs, one file for each page of content
        /// (instead of a single TIFF with multiple pages).
        /// <value><see langword="true"/> if the output should be multiple TIFF
        /// files, one file for each page of content; <see langword="false"/> if
        /// the output should be a single TIFF file with multiple pages. The
        /// default value is <see langword="false"/>.</value>
        /// </summary>
        public bool ForceOneFilePerPage { get; set; }

        /// <summary>
        /// Gets or sets the maximum pixel width of the output TIFF(s),
        /// expressed as a CSS-style string, e.g. "800px". When specified, the
        /// output image is guaranteed to never be wider than the specified
        /// value and its aspect ratio will be preserved. This is useful if you
        /// need all of your output images to fit within a single column.
        /// </summary>
        public string MaxWidth { get; set; }

        /// <summary>
        /// Gets or sets the maximum pixel height of the output TIFF(s),
        /// expressed as a CSS-style string, e.g. "600px". When specified, the
        /// output image is guaranteed to never be taller than the specified
        /// value and its aspect ratio will be preserved. This is useful if you
        /// need all of your output images to fit within a single row.
        /// </summary>
        public string MaxHeight { get; set; }
    }
}
