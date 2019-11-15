using System;
using Accusoft.PrizmDoc.Net.Http;

namespace Accusoft.PrizmDocServer
{
    /// <summary>
    /// Represents a remote PrizmDoc Server deployment you want to use.
    /// This is the main class you need to instantiate to use this SDK.
    /// </summary>
    public partial class PrizmDocServerClient
    {
        private readonly PrizmDocRestClient restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrizmDocServerClient"/>
        /// class, allowing you to work with a PrizmDoc Server deployment at the
        /// specified <paramref name="baseAddress"/>.
        /// </summary>
        /// <param name="baseAddress">Base URL for connecting to your PrizmDoc
        /// Server deployment (e.g. <c>"http://localhost:18681"</c> or
        /// <c>"https://my-load-balancer"</c>).</param>
        public PrizmDocServerClient(string baseAddress)
            : this(baseAddress, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrizmDocServerClient"/>
        /// class, allowing you to work with a PrizmDoc Server environment at
        /// the specified <paramref name="baseAddress"/>.
        /// </summary>
        /// <param name="baseAddress">Base URL for connecting to your PrizmDoc
        /// Server deployment (e.g. <c>"http://localhost:18681"</c> or
        /// <c>"https://my-load-balancer"</c>).</param>
        public PrizmDocServerClient(Uri baseAddress)
            : this(baseAddress, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrizmDocServerClient"/>
        /// class, allowing you to work with <see
        /// href="https://cloud.accusoft.com">PrizmDoc Cloud</see>.
        /// </summary>
        /// <param name="baseAddress">Base URL to PrizmDoc Server (e.g.
        /// <c>"https://api.accusoft.com"</c>).</param>
        /// <param name="apiKey">Your <see
        /// href="https://cloud.accusoft.com">PrizmDoc Cloud</see> API
        /// key.</param>
        public PrizmDocServerClient(string baseAddress, string apiKey)
            : this(new Uri(baseAddress), apiKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrizmDocServerClient"/>
        /// class, allowing you to work with <see
        /// href="https://cloud.accusoft.com">PrizmDoc Cloud</see>.
        /// </summary>
        /// <param name="baseAddress">Base URL to PrizmDoc Server (e.g.
        /// <c>"https://api.accusoft.com"</c>).</param>
        /// <param name="apiKey">Your <see
        /// href="https://cloud.accusoft.com">PrizmDoc Cloud</see> API
        /// key.</param>
        public PrizmDocServerClient(Uri baseAddress, string apiKey)
        {
            this.restClient = new PrizmDocRestClient(baseAddress);

            if (apiKey != null)
            {
                this.restClient.DefaultRequestHeaders.Add("Acs-Api-Key", apiKey);
            }
        }
    }
}
