namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// PDF-specific conversion output options.
    /// </summary>
    public class PdfDestinationOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the output should be
        /// multiple single-page PDFs, one file for each page of content
        /// (instead of a single PDF with multiple pages).
        /// <value><c>true</c> if the output should be multiple PDF
        /// files, one file for each page of content; <c>false</c> if
        /// the output should be a single PDF file with multiple pages. The
        /// default value is <c>false</c>.</value>
        /// </summary>
        public bool ForceOneFilePerPage { get; set; }

        /// <summary>
        /// Gets or sets options to use if OCR should be performed. When set,
        /// OCR will be performed on image-based source documents to produce a
        /// text-searchable PDF. When null, no OCR will be performed. Default is
        /// null.
        /// </summary>
        public OcrOptions Ocr { get; set; }
    }
}
