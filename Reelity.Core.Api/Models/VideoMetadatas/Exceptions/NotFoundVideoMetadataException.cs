using System;
using Xeptions;

namespace Reelity.Core.Api.Models.VideoMetadatas.Exceptions
{
    public class NotFoundVideoMetadataException: Xeption
    {
        public NotFoundVideoMetadataException(string message)
            : base(message: message)
        { }
    }
}
