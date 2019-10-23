using System;
using System.Collections.Generic;

namespace Accusoft.PrizmDocServer.Conversion
{
  /// <summary>
  /// Represents a conversion result, either a successful result which can be downloaded as a file or an error result if a page or set of pages could not be converted.
  /// </summary>
  public class Result
  {
    private readonly RemoteWorkFile remoteWorkFile;

    internal Result(RemoteWorkFile remoteWorkFile, int pageCount, IEnumerable<SourceDocument> sources)
    {
      if (remoteWorkFile == null)
      {
        throw new ArgumentNullException("remoteWorkFile");
      }

      if (sources == null)
      {
        throw new ArgumentNullException("sources");
      }

      this.remoteWorkFile = remoteWorkFile;
      this.PageCount = pageCount;
      this.Sources = sources;
    }

    internal Result(string errorCode, IEnumerable<SourceDocument> sources)
    {
      this.ErrorCode = errorCode;
      this.Sources = sources;
    }

    /// <summary>
    /// If <c>true</c>, this is a successful result and you can use <see cref="RemoteWorkFile"/> to download this particular output file.
    /// If <c>false</c>, this is an error result. See the <see cref="ErrorCode"/> for more information.
    /// </summary>
    public bool IsSuccess { get { return !IsError; } }

    /// <summary>
    /// If <c>true</c>, this is an error result. See the <see cref="ErrorCode"/> for more information.
    /// If <c>false</c>, this is a successful result and you can use <see cref="RemoteWorkFile"/> to download this particular output file.
    /// </summary>
    public bool IsError { get { return ErrorCode != null; } }

    /// <summary>
    /// Specific error code if this is an error result, <c>null</c> if this is a successful result.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// <see cref="RemoteWorkFile"/> for a successful result which can be saved locally.
    /// </summary>
    public RemoteWorkFile RemoteWorkFile
    {
      get
      {
        // We want to throw if someone tries to get the RemoteWorkFile from an error result,
        // because we don't want them storing null to some var and then use it much later
        // only to discover they can't. So we fail fast.
        if (IsError)
        {
          throw new InvalidOperationException("This result represents an error instead of a success. It has no RemoteWorkFile. Make sure you check IsSuccess is true before accessing the RemoteWorkFile property.");
        }

        return remoteWorkFile;
      }
    }

    /// <summary>
    /// For a successful result, the total number of pages in this output document.
    /// <c>null</c> for an error result.
    /// </summary>
    public int? PageCount { get; }

    /// <summary>
    /// Collection of source documents which contributed to this specific result.
    /// </summary>
    public IEnumerable<SourceDocument> Sources { get; }
  }
}
