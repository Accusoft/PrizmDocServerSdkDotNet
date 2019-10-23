namespace Accusoft.PrizmDocServer.Conversion
{
  public class OcrOptions
  {
    public OcrOptions()
    {
      Language = "english";
    }

    /// <summary>
    /// OCR language. Default is "english".
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Default DPI to use whenever an input does not itself specify any DPI information.
    /// </summary>
    public DpiOptions DefaultDpi { get; set; }
  }
}
