namespace Accusoft.PrizmDocServer.Conversion
{
  public class JpegDestinationOptions
  {
    /// <summary>
    /// Maximum pixel width of the output JPEG(s), expressed as a CSS-style
    /// string, e.g. "800px". When specified, the output image is guaranteed to
    /// never be wider than the specified value and its aspect ratio will be
    /// preserved. This is useful if you need all of your output images to fit
    /// within a single column.
    /// </summary>
    public string MaxWidth { get; set; }

    /// <summary>
    /// Maximum pixel height of the output JPEG(s), expressed as a CSS-style
    /// string, e.g. "600px". When specified, the output image is guaranteed to
    /// never be taller than the specified value and its aspect ratio will be
    /// preserved. This is useful if you need all of your output images to fit
    /// within a single row.
    /// </summary>
    public string MaxHeight { get; set; }
  }
}
