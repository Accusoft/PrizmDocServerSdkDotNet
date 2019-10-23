namespace Accusoft.PrizmDocServer.Conversion
{
  public class DestinationOptions
  {
    /// <summary>
    /// Creates a new <see cref="DestinationOptions"/> instance.
    /// </summary>
    /// <param name="format">File format to convert to.</param>
    public DestinationOptions(DestinationFileFormat format)
    {
      Format = format;
    }

    /// <summary>
    /// File format to convert to.
    /// </summary>
    public DestinationFileFormat Format { get; }

    /// <summary>
    /// JPEG-specific options, applied when the destination file format is JPEG.
    /// </summary>
    public JpegDestinationOptions JpegOptions { get; set; }

    /// <summary>
    /// PDF-specific options, applied when the destination file format is PDF.
    /// </summary>
    public PdfDestinationOptions PdfOptions { get; set; }

    /// <summary>
    /// PNG-specific options, applied when the destination file format is PNG.
    /// </summary>
    public PngDestinationOptions PngOptions { get; set; }

    /// <summary>
    /// TIFF-specific options, applied when the destination file format is TIFF.
    /// </summary>
    public TiffDestinationOptions TiffOptions { get; set; }

    /// <summary>
    /// Header to be appended to each page of the output document. The original page content will be left unaltered. The overall page dimensions will be expanded to accommodate the header content.
    /// </summary>
    public HeaderFooterOptions Header { get; set; }

    /// <summary>
    /// Footer to be appended to each page of the output document. The original page content will be left unaltered. The overall page dimensions will be expanded to accommodate the footer content.
    /// </summary>
    /// <value></value>
    public HeaderFooterOptions Footer { get; set; }
  }
}
