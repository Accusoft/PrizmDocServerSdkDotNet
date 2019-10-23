using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Conversion;
using SixLabors.ImageSharp;

namespace Accusoft.PrizmDocServer.Tests
{
  public class ImageDimensions
  {
    public int Width { get; set; }
    public int Height { get; set; }
  }

  public static class ImageUtil
  {
    public static ImageDimensions GetImageDimensions(string filename)
    {
      using (var file = File.OpenRead(filename))
      {
        return GetImageDimensions(file);
      }
    }

    public static ImageDimensions GetImageDimensions(Stream stream)
    {
      if (stream.Position > 0)
      {
        stream.Position = 0;
      }

      var info = Image.Identify(stream);

      if (info == null)
      {
        throw new ArgumentException("Could not determine image information");
      }

      return new ImageDimensions() { Width = info.Width, Height = info.Height };
    }

    public static async Task<IEnumerable<ImageDimensions>> GetTiffPagesDimensionsAsync(string filename)
    {
      var context = Util.CreateContext();
      var results = await context.ConvertAsync(filename, DestinationFileFormat.Png);

      IList<ImageDimensions> pageDimensions = new List<ImageDimensions>();

      foreach (var result in results)
      {
        using (var memoryStream = new MemoryStream())
        {
          await result.RemoteWorkFile.CopyToAsync(memoryStream);
          var dimensions = GetImageDimensions(memoryStream);
          pageDimensions.Add(dimensions);
        }
      }

      return pageDimensions;
    }
  }
}
