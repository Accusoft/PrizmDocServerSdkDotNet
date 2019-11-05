namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// Output file format to use when performing a document conversion.
    /// </summary>
    public enum DestinationFileFormat
    {
        /// <summary>JPEG output. Because the JPEG file format only supports a single page, multiple JPEG files will be created for each page of output.</summary>
        Jpeg,

        /// <summary>PDF output. Will be a single PDF file with multiple pages unless <see cref="PdfDestinationOptions.ForceOneFilePerPage"/> is set to <c>true</c>.</summary>
        Pdf,

        /// <summary>PNG output. Because the PNG file format only supports a single page, multiple PNG files will be created for each page of output.</summary>
        Png,

        /// <summary>SVG output. Because the SVG file format only supports a single page, multiple SVG files will be created for each page of output.</summary>
        Svg,

        /// <summary>TIFF output. Will be a single TIFF file with multiple pages unless <see cref="TiffDestinationOptions.ForceOneFilePerPage"/> is set to <c>true</c>.</summary>
        Tiff,

        /// <summary>DOCX output. Will be a single DOCX file with multiple pages.</summary>
        Docx,
    }
}
