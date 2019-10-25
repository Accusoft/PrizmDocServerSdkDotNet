using System;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;

namespace Accusoft.PrizmDocServer.Conversion
{
  /// <summary>
  /// Defines a document or pages of a document which should be used as input
  /// for a conversion.
  ///
  /// Typically, you create a source document from a local file path, like this:
  ///
  /// <code>
  /// var sourceDocument = new SourceDocument("my-local-file.docx");
  /// </code>
  ///
  /// You can optionally specify a specific set of pages to use:
  ///
  /// <code>
  /// var sourceDocument = new SourceDocument("my-local-file.docx", pages: "2, 5-9, 14-");
  /// </code>
  ///
  /// And, if the document is password-protected, you can optionally specify the
  /// password required to open it:
  ///
  /// <code>
  /// var sourceDocument = new SourceDocument("secret.docx", password: "opensesame");
  /// </code>
  ///
  /// Finally, you can create a source document from an already-existing remote
  /// work file, like so:
  ///
  /// <code>
  /// var result = await prizmDocServer.ConvertToPdfAsync("my-local-file.docx");
  /// var sourceDocument = new SourceDocument(result.RemoteWorkFile);
  /// </code>
  ///
  /// In this way, you can use the results of one operation as input to a
  /// subsequent operation without needing to download the intermediate results.
  /// </summary>
  public class SourceDocument
  {
    /// <summary>
    /// Creates a new SourceDocument instance for a local file.
    /// </summary>
    /// <param name="localFilePath">Local file to use as a source document.</param>
    /// <param name="pages">When provided, causes the conversion to only use a specified set of pages from the source document.
    /// Page numbers are 1-indexed.
    /// You can think of this argument like a "pages" input text field in a typical print dialog box.
    /// For example, the value can be a single page like <c>"2"</c>,
    /// a comma-delimited list of specific pages like <c>"1, 4, 5"</c>,
    /// an open-ended page range like <c>"2-"</c> (page 2 through the end of the document),
    /// or a combination of these, like <c>"2, 4-9, 12-"</c>.
    /// </param>
    /// <param name="password">Password to open the document. Only required if the document requires a password to open.</param>
    public SourceDocument(string localFilePath, string pages = null, string password = null)
    {
      this.LocalFilePath = localFilePath;
      this.Pages = pages;
      this.Password = password;
    }

    /// <summary>
    /// Creates a new SourceDocument instance for an existing remote work file.
    /// </summary>
    /// <param name="remoteWorkFile">Remote work file to use as a source document.</param>
    /// <param name="pages">When provided, causes the conversion to only use a specified set of pages from the source document.
    /// Page numbers are 1-indexed.
    /// You can think of this argument like a "pages" input text field in a typical print dialog box.
    /// For example, the value can be a single page like <c>"2"</c>,
    /// a comma-delimited list of specific pages like <c>"1, 4, 5"</c>,
    /// an open-ended page range like <c>"2-"</c> (page 2 through the end of the document),
    /// or a combination of these, like <c>"2, 4-9, 12-"</c>.
    /// </param>
    /// <param name="password">Password to open the document. Only required if the document requires a password to open.</param>
    public SourceDocument(RemoteWorkFile remoteWorkFile, string pages = null, string password = null)
    {
      if (remoteWorkFile == null)
      {
        throw new ArgumentNullException("remoteWorkFile");
      }

      this.RemoteWorkFile = remoteWorkFile;
      this.Pages = pages;
      this.Password = password;
    }

    /// <summary>
    /// Local file path associated with this source document or <c>null</c> if this source document is not associated with a local file path.
    /// </summary>
    public string LocalFilePath { get; }

    /// <summary>
    /// Associated remote work file for this source document or <c>null</c> if the remote work file has not yet been created.
    /// </summary>
    public RemoteWorkFile RemoteWorkFile { get; private set; }

    /// <summary>
    /// Specific pages which should be used or <c>null</c> if the entire document should be used.
    /// </summary>
    public string Pages { get; }

    /// <summary>
    /// Password to open the document or <c>null</c> if no password was provided.
    /// </summary>
    internal string Password { get; }

    /// <summary>
    /// Ensures the RemoteWorkFile property is assigned a RemoteWorkFile instance with the given affinityToken.
    /// </summary>
    internal async Task EnsureUsableRemoteWorkFileAsync(AffinitySession affinitySession, string affinityToken = null)
    {
      if (RemoteWorkFile != null)
      {
        // If no affinity token was specified, there is nothing to do.
        if (affinityToken == null)
        {
          return;
        }

        // If the RemoteWorkFile already exists and has the correct affinity,
        // there is nothing to do.
        if (RemoteWorkFile.AffinityToken == affinityToken)
        {
          return;
        }

        // If the remote work file exists but has the wrong affinity (it is on
        // the wrong machine), then download and re-upload it to the correct
        // machine, and assign the new RemoteWorkFile to this SourceDocument.
        using(var res = await RemoteWorkFile.HttpGetAsync())
        {
          await res.ThrowIfRestApiError();
          var downloadStream = await res.Content.ReadAsStreamAsync();
          RemoteWorkFile = await affinitySession.UploadAsync(downloadStream, affinityToken: affinityToken);
        }

        return;
      }

      // If the RemoteWorkFile does NOT exist, then we need to upload the
      // specified local file...

      // If there is no actual local file to upload, then fail.
      if (!File.Exists(LocalFilePath))
      {
        throw new FileNotFoundException($"File not found: {LocalFilePath}");
      }

      // Upload the local file using the specified affinity token.
      RemoteWorkFile = await affinitySession.UploadAsync(LocalFilePath, affinityToken: affinityToken);
    }
  }
}
