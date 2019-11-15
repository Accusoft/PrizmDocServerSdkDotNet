using System.IO;
using System.Text;

namespace Accusoft.PrizmDocServer.Tests
{
    public static class FileUtil
    {
        public static bool IsPdf(string filename)
        {
            var header = ReadFileHeader(filename, 5);

            return header == Encoding.ASCII.GetBytes("%PDF-");
        }

        public static byte[] ReadFileHeader(string filename, int numBytes)
        {
            byte[] buffer = new byte[numBytes];

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                fs.Read(buffer, 0, buffer.Length);
            }

            return buffer;
        }
    }
}
