using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
    public static class FileAssert
    {
        public static void IsPdf(string filename)
        {
            CollectionAssert.AreEqual(Encoding.ASCII.GetBytes("%PDF-"), FileUtil.ReadFileHeader(filename, 5), "File header bytes did not indicate PDF.");
        }

        public static void IsTiff(string filename)
        {
            byte[] header = FileUtil.ReadFileHeader(filename, 4);

            // See https://www.adobe.io/content/dam/udp/en/open/standards/tiff/TIFF6.pdf
            if (header[0] == (byte)'I')
            { // Intel (II), little-endian.
                CollectionAssert.AreEqual(new byte[] { (byte)'I', (byte)'I', 42, 0 }, header, "File header bytes did not indicate TIFF.");
            }
            else
            { // Motorola (MM), big-endian.
                CollectionAssert.AreEqual(new byte[] { (byte)'M', (byte)'M', 0, 42 }, header, "File header bytes did not indicate TIFF.");
            }
        }

        public static void IsPng(string filename)
        {
            byte[] header = FileUtil.ReadFileHeader(filename, 8);

            // See http://www.libpng.org/pub/png/spec/1.2/PNG-Rationale.html#R.PNG-file-signature
            CollectionAssert.AreEqual(new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G', (byte)'\r', (byte)'\n', 0x1a /* ctrl-z */, (byte)'\n' }, header, "File header bytes did not indicate PNG.");
        }

        public static void IsJpeg(string filename)
        {
            byte[] header = FileUtil.ReadFileHeader(filename, 3);

            // See https://en.wikipedia.org/wiki/JPEG_File_Interchange_Format#File_format_structure
            CollectionAssert.AreEqual(new byte[] { 0xFF, 0xD8, 0xFF }, header, "File header bytes did not indicate JPEG.");
        }

        public static void IsSvg(string filename)
        {
            byte[] header = FileUtil.ReadFileHeader(filename, 4);

            // See https://en.wikipedia.org/wiki/JPEG_File_Interchange_Format#File_format_structure
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes("<svg"), header, "File contents did not begin with \"<svg\".");
        }
    }
}
