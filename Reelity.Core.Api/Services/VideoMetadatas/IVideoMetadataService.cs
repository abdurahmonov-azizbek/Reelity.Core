// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Api.Models.VideoMetadatas;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Reelity.Core.Api.Services.VideoMetadatas
{
    public interface IVideoMetadataService
    {
        ValueTask<VideoMetadata> AddVideoMetadataAsync(VideoMetadata videoMetadata);
        IQueryable<VideoMetadata> RetrieveAllVideoMetadatas();
        ValueTask<VideoMetadata> RetrieveVideoMetadataByIdAsync(Guid videoMetadataId);
    }
}
