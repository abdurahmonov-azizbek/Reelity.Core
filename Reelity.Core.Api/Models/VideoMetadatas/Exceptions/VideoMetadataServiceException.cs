﻿using Xeptions;

namespace Reelity.Core.Api.Models.VideoMetadatas.Exceptions
{
    public class VideoMetadataServiceException : Xeption
    {
        public VideoMetadataServiceException(string message, Xeption innerException) 
            : base(message, innerException)
        { }
    }
}
