// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Api.Models.Metadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;

namespace Reelity.Core.Api.Services.VideoMetadatas
{
    public partial class VideoMetadataService
    {
        private void ValidateVideoMetadataOnAdd(VideoMetadata videoMetadata)
        {
            ValidateVideoMetadataNotNull(videoMetadata);
        }

        private void ValidateVideoMetadataNotNull(VideoMetadata videoMetadata)
        {
            if (videoMetadata is null)
            {
                throw new NullVideoMetadataException("VideoMetadata is null.");
            }
        }
    }
}
