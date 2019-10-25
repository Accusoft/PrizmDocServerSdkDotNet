using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;

namespace Accusoft.PrizmDocServer
{
  public class RemoteWorkFile : IEquatable<RemoteWorkFile>
  {
    internal RemoteWorkFile(AffinitySession session, string fileId, string affinityToken, string fileExtension)
    {
      this.session = session;
      this.FileId = fileId;
      this.AffinityToken = affinityToken;
      this.FileExtension = fileExtension;
    }

    private AffinitySession session;

    public string FileId { get; }
    public string AffinityToken { get; }
    public string FileExtension { get; }

    public async Task SaveAsync(string localFilePath)
    {
      using (var localFileWriteStream = File.OpenWrite(localFilePath))
      {
        await CopyToAsync(localFileWriteStream);
      }
    }

    public async Task CopyToAsync(Stream stream)
    {
      using (var res = await session.GetAsync($"/PCCIS/V1/WorkFile/{FileId}"))
      {
        await res.ThrowIfRestApiError();
        await res.Content.CopyToAsync(stream);
      }
    }

    internal async Task<HttpResponseMessage> HttpGetAsync()
    {
      return await session.GetAsync($"/PCCIS/V1/WorkFile/{FileId}");
    }

    public bool Equals(RemoteWorkFile other)
    {
      return this.FileId == other.FileId &&
             this.AffinityToken == other.AffinityToken &&
             this.FileExtension == other.FileExtension;
    }

    public override int GetHashCode()
    {
      var hashCode = 3681487;
      hashCode = hashCode * -1521134295 + EqualityComparer<AffinitySession>.Default.GetHashCode(session);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileId);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AffinityToken);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileExtension);
      return hashCode;
    }

    public override bool Equals(object obj)
    {
      var other = obj as RemoteWorkFile;

      if (other == null)
      {
        return false;
      }

      return this.Equals(other);
    }

    public override string ToString()
    {
      var builder = new StringBuilder();

      builder.Append("RemoteWorkFile { ");
      builder.Append($"FileId: {FileId}");
      if (AffinityToken != null)
      {
        builder.Append($", AffinityToken: {AffinityToken}");
      }
      if (FileExtension != null)
      {
        builder.Append($", FileExtension: {FileExtension}");
      }
      builder.Append(" }");

      return builder.ToString();
    }
  }
}
