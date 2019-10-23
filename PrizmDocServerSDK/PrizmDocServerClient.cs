using System;
using Accusoft.PrizmDoc.Net.Http;

namespace Accusoft.PrizmDocServer
{
  /// <summary>
  /// Defines how to connect to a remote PrizmDoc Server environment you want to use.
  /// </summary>
  public class PrizmDocServerClient
  {
    internal readonly PrizmDocRestClient client;

    /// <summary>
    /// Creates a <see cref="PrizmDocServerClient"/> instance for working with a PrizmDoc Server environment at the specified <paramref name="baseAddress"/>.
    /// </summary>
    /// <param name="baseAddress">Base URL for connecting to your PrizmDoc Server deployment (e.g. "http://localhost:18681" or "https://my-load-balancer").</param>
    public PrizmDocServerClient(string baseAddress) : this(baseAddress, null) { }

    /// <summary>
    /// Creates a <see cref="PrizmDocServerClient"/> instance for working with a PrizmDoc Server environment at the specified <paramref name="baseAddress"/>.
    /// </summary>
    /// <param name="baseAddress">Base URL for connecting to your PrizmDoc Server deployment (e.g. "http://localhost:18681" or "https://my-load-balancer").</param>
    public PrizmDocServerClient(Uri baseAddress) : this(baseAddress, null) { }

    /// <summary>
    /// Creates a <see cref="PrizmDocServerClient"/> instance for working with <see href="https://cloud.accusoft.com">PrizmDoc Cloud</see>.
    /// </summary>
    /// <param name="baseAddress">Base URL to PrizmDoc Server (e.g. "https://api.accusoft.com").</param>
    /// <param name="apiKey">Your <see href="https://cloud.accusoft.com">PrizmDoc Cloud</see> API key.</param>
    public PrizmDocServerClient(string baseAddress, string apiKey) : this(new Uri(baseAddress), apiKey) { }

    /// <summary>
    /// Creates a <see cref="PrizmDocServerClient"/> instance for working with <see href="https://cloud.accusoft.com">PrizmDoc Cloud</see>.
    /// </summary>
    /// <param name="baseAddress">Base URL to PrizmDoc Server (e.g. "https://api.accusoft.com").</param>
    /// <param name="apiKey">Your <see href="https://cloud.accusoft.com">PrizmDoc Cloud</see> API key.</param>
    public PrizmDocServerClient(Uri baseAddress, string apiKey)
    {
      client = new PrizmDocRestClient(baseAddress);

      if (apiKey != null)
      {
        client.DefaultRequestHeaders.Add("Acs-Api-Key", apiKey);
      }
    }

    /// <summary>
    /// Creates a new <see cref="ProcessingContext"/> which you can use to do
    /// actual work. You should create a new <see cref="ProcessingContext"/>
    /// each time you need to perform some new set of work on one or more local
    /// documents.
    /// </summary>
    public ProcessingContext CreateProcessingContext()
    {
      return new ProcessingContext(client.CreateAffinitySession());
    }
  }
}
