#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1300 // Element should begin with upper-case letter

namespace Accusoft.PrizmDocServer.Json.Deserialization
{
    public class PostWorkFileResponse
    {
        public string fileId { get; set; }

        public string fileExtension { get; set; }

        public string affinityToken { get; set; }
    }
}
