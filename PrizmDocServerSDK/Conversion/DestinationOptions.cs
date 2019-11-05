namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// Conversion output options.
    /// </summary>
    public class DestinationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestinationOptions"/> class.
        /// </summary>
        /// <param name="format">File format to convert to.</param>
        public DestinationOptions(DestinationFileFormat format)
        {
            this.Format = format;
        }

        /// <summary>
        /// Gets file format to convert to.
        /// </summary>
        public DestinationFileFormat Format { get; }

        /// <summary>
        /// Gets or sets JPEG-specific options, applied when the destination file format is JPEG.
        /// </summary>
        public JpegDestinationOptions JpegOptions { get; set; }

        /// <summary>
        /// Gets or sets PDF-specific options, applied when the destination file format is PDF.
        /// </summary>
        public PdfDestinationOptions PdfOptions { get; set; }

        /// <summary>
        /// Gets or sets PNG-specific options, applied when the destination file format is PNG.
        /// </summary>
        public PngDestinationOptions PngOptions { get; set; }

        /// <summary>
        /// Gets or sets TIFF-specific options, applied when the destination file format is TIFF.
        /// </summary>
        public TiffDestinationOptions TiffOptions { get; set; }

        /// <summary>
        /// Gets or sets the header options which define the header to be appended to each page of the output document. The original page content will be left unaltered. The overall page dimensions will be expanded to accommodate the header content.
        /// </summary>
        public HeaderFooterOptions Header { get; set; }

        /// <summary>
        /// Gets or sets the footer options which define the footer to be appended to each page of the output document. The original page content will be left unaltered. The overall page dimensions will be expanded to accommodate the footer content.
        /// </summary>
        public HeaderFooterOptions Footer { get; set; }
    }
}
