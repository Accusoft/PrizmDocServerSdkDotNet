namespace Accusoft.PrizmDocServer.Conversion
{
  public class PdfDestinationOptions
  {
    /// <summary>
    /// When <see langword="true"/>, produce single-page PDF files, one file
    /// for each page of content (instead of a single PDF with multiple pages).
    /// Default is <see langword="false"/>.
    /// </summary>
    public bool ForceOneFilePerPage { get; set; }

    /// <summary>
    /// When set, OCR will be performed on image-based source documents to
    /// produce a text-searchable PDF. When null, no OCR will be performed.
    /// Default is null.
    /// </summary>
    public OcrOptions Ocr { get; set; }
  }
}
