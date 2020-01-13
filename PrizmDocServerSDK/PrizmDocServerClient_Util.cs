using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Documented in PrizmDocServerClient.cs")]
    public partial class PrizmDocServerClient
    {
        private string GetAt(string rawErrorDetails)
        {
            string at = string.Empty;
            try
            {
                at = (string)JObject.Parse(rawErrorDetails)["at"];
            }
            catch
            {
            }

            return at;
        }
    }
}
