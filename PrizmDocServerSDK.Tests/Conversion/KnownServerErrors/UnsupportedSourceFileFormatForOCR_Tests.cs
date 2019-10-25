using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using Accusoft.PrizmDocServer.Exceptions;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
  [TestClass]
  public class UnsupportedSourceFileFormatForOCR_Tests
  {
    [TestMethod]
    public async Task When_using_a_single_source_input()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      // This assertion is coupled to what is currently supported in the product. It would be better if we used a mock for this test.
      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync(new SourceDocument("documents/example.docx"), new DestinationOptions(DestinationFileFormat.Pdf)
        {
          PdfOptions = new PdfDestinationOptions()
          {
            Ocr = new OcrOptions()
            {
              Language = "english"
            }
          }
        });
      }, "Unsupported file format when performing OCR, \"docx\". The remote server only supports the following input file formats when performing OCR: \"bmp\", \"cal\", \"cals\", \"cur\", \"cut\", \"dcim\", \"dcm\", \"dcx\", \"dib\", \"dicm\", \"dicom\", \"emz\", \"gif\", \"ico\", \"img\", \"jp2\", \"jpc\", \"jpeg\", \"jpg\", \"jpx\", \"ncr\", \"pbm\", \"pcd\", \"pct\", \"pcx\", \"pdf\", \"pgm\", \"pic\", \"pict\", \"png\", \"ppm\", \"psb\", \"psd\", \"ras\", \"sct\", \"sgi\", \"tga\", \"tif\", \"tiff\", \"tpic\", \"wbmp\", \"wmz\", \"wpg\", \"xbm\", \"xwd\"");
    }
  }
}
