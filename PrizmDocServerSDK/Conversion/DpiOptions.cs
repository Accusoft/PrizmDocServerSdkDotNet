using System;

namespace Accusoft.PrizmDocServer.Conversion
{
  public class DpiOptions
  {
    /// <param name="x">Horizontal DPI.</param>
    /// <param name="y">Vertical DPI.</param>
    public DpiOptions(int x, int y)
    {
      if (x <= 0)
      {
        throw new ArgumentOutOfRangeException("x", "value must be greater than zero");
      }

      if (y <= 0)
      {
        throw new ArgumentOutOfRangeException("y", "value must be greater than zero");
      }

      X = x;
      Y = y;
    }

    /// <summary>
    /// Horizontal DPI.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Vertical DPI.
    /// </summary>
    public int Y { get; }
  }
}
