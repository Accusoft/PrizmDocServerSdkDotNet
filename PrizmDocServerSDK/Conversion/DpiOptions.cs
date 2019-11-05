using System;

namespace Accusoft.PrizmDocServer.Conversion
{
    /// <summary>
    /// DPI options.
    /// </summary>
    public class DpiOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DpiOptions"/> class.
        /// </summary>
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

            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets horizontal DPI.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets vertical DPI.
        /// </summary>
        public int Y { get; }
    }
}
