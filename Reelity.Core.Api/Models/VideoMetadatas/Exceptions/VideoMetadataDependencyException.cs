// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Xeptions;

namespace Reelity.Core.Api.Models.VideoMetadatas.Exceptions
{
    public class VideoMetadataDependencyException : Xeption
    {
        public VideoMetadataDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
