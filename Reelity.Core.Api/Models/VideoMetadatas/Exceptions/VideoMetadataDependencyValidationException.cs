// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using System;
using Xeptions;

namespace Reelity.Core.Api.Models.VideoMetadatas.Exceptions
{
    public class VideoMetadataDependencyValidationException : Xeption
    {
        public VideoMetadataDependencyValidationException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        { }
    }
}
