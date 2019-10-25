using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accusoft.PrizmDocServer.Tests;
using Accusoft.PrizmDocServer.Exceptions;
using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion.KnownServerErrors.Tests
{
  [TestClass]
  public class UnsupportedConversion_Tests
  {
    [TestMethod]
    public async Task When_using_a_single_source_input()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      // This assertion is coupled to what is currently supported in the product. It would be better if we used a mock for this test.
      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync(new SourceDocument("documents/example.mp3"), new DestinationOptions(DestinationFileFormat.Pdf));
      }, "Unsupported file format \"mp3\". The remote server only supports the following input file formats: \"bmp\", \"cal\", \"cals\", \"csv\", \"cur\", \"cut\", \"dcim\", \"dcm\", \"dcx\", \"dgn\", \"dib\", \"dicm\", \"dicom\", \"doc\", \"docm\", \"docx\", \"dot\", \"dotm\", \"dotx\", \"dwf\", \"dwg\", \"dxf\", \"eml\", \"emz\", \"fodg\", \"fodp\", \"fods\", \"fodt\", \"gif\", \"htm\", \"html\", \"ico\", \"img\", \"jp2\", \"jpc\", \"jpeg\", \"jpg\", \"jpx\", \"msg\", \"ncr\", \"odg\", \"odp\", \"ods\", \"odt\", \"otg\", \"otp\", \"ots\", \"ott\", \"pbm\", \"pcd\", \"pct\", \"pcx\", \"pdf\", \"pgm\", \"pic\", \"pict\", \"png\", \"pot\", \"potm\", \"potx\", \"ppm\", \"pps\", \"ppsm\", \"ppsx\", \"ppt\", \"pptm\", \"pptx\", \"psb\", \"psd\", \"ras\", \"rtf\", \"sct\", \"sgi\", \"tga\", \"tif\", \"tiff\", \"tpic\", \"txt\", \"vdx\", \"vsd\", \"vsdm\", \"vsdx\", \"wbmp\", \"wmf\", \"wmz\", \"wpg\", \"xbm\", \"xhtml\", \"xls\", \"xlsm\", \"xlsx\", \"xlt\", \"xltm\", \"xltx\", \"xwd\"");
    }

    [TestMethod]
    public async Task When_there_are_multiple_source_inputs()
    {
      var prizmDocServer = Util.CreatePrizmDocServerClient();

      // This assertion is coupled to what is currently supported in the product. It would be better if we used a mock for this test.
      await UtilAssert.ThrowsExceptionWithMessageAsync<RestApiErrorException>(async () =>
      {
        await prizmDocServer.ConvertAsync(new List<SourceDocument> {
          new SourceDocument("documents/example.pdf"),
          new SourceDocument("documents/example.mp3"),
          new SourceDocument("documents/example.pdf"),
        }, new DestinationOptions(DestinationFileFormat.Pdf));
      }, "Unsupported file format \"mp3\" for SourceDocument at index 1. The remote server only supports the following input file formats: \"bmp\", \"cal\", \"cals\", \"csv\", \"cur\", \"cut\", \"dcim\", \"dcm\", \"dcx\", \"dgn\", \"dib\", \"dicm\", \"dicom\", \"doc\", \"docm\", \"docx\", \"dot\", \"dotm\", \"dotx\", \"dwf\", \"dwg\", \"dxf\", \"eml\", \"emz\", \"fodg\", \"fodp\", \"fods\", \"fodt\", \"gif\", \"htm\", \"html\", \"ico\", \"img\", \"jp2\", \"jpc\", \"jpeg\", \"jpg\", \"jpx\", \"msg\", \"ncr\", \"odg\", \"odp\", \"ods\", \"odt\", \"otg\", \"otp\", \"ots\", \"ott\", \"pbm\", \"pcd\", \"pct\", \"pcx\", \"pdf\", \"pgm\", \"pic\", \"pict\", \"png\", \"pot\", \"potm\", \"potx\", \"ppm\", \"pps\", \"ppsm\", \"ppsx\", \"ppt\", \"pptm\", \"pptx\", \"psb\", \"psd\", \"ras\", \"rtf\", \"sct\", \"sgi\", \"tga\", \"tif\", \"tiff\", \"tpic\", \"txt\", \"vdx\", \"vsd\", \"vsdm\", \"vsdx\", \"wbmp\", \"wmf\", \"wmz\", \"wpg\", \"xbm\", \"xhtml\", \"xls\", \"xlsm\", \"xlsx\", \"xlt\", \"xltm\", \"xltx\", \"xwd\"");
    }
  }
}
