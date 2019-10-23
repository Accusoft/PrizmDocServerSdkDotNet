using System.IO;
using System.Text;

namespace Accusoft.PrizmDocServer.Tests
{
  public static class FileUtil
  {
    public static bool IsPdf(string filename)
    {
      var header = ReadFileHeader(filename, 5);

      return header == ASCIIEncoding.ASCII.GetBytes("%PDF-");
    }

    public static byte[] ReadFileHeader(string filename, int numBytes)
    {
      var buffer = new byte[numBytes];

      using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
      {
        fs.Read(buffer, 0, buffer.Length);
      }

      return buffer;
    }
  }
}
