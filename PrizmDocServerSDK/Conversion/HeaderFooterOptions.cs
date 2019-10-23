using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion
{
  /// <summary>
  /// Defines header or footer content to be applied.
  /// </summary>
  public class HeaderFooterOptions
  {
    /// <summary>
    /// Collection of header/footer lines.
    /// Each line may define text in the left, center, and/or right position.
    /// </summary>
    public List<HeaderFooterLine> Lines { get; set; }

    /// <summary>
    /// Font family to use. The font family must be available on the remote server.
    /// </summary>
    public string FontFamily { get; set; }

    /// <summary>
    /// Font size in points. Value must be a string with a number followed by "pt", e.g. "12.pt".
    /// </summary>
    public string FontSize { get; set; }

    /// <summary>
    /// Color of the text. Value must be a CSS hex-based color value (like "#FF0000").
    /// </summary>
    public string Color { get; set; }
  }
}
