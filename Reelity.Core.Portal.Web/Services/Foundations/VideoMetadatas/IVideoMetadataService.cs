// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Portal.Web.Models.VideoMetadatas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reelity.Core.Portal.Web.Services.Foundations.VideoMetadatas
{
    public interface IVideoMetadataService
    {
        ValueTask<VideoMetadata> AddVideoMetadataAsync(VideoMetadata videoMetadata);
        ValueTask<List<VideoMetadata>> RetrieveAllVideoMetadatasAsync();
    }
}
