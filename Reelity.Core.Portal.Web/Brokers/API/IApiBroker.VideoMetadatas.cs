// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Portal.Web.Models.VideoMetadatas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reelity.Core.Portal.Web.Brokers.API
{
    public partial interface IApiBroker
    {
        ValueTask<VideoMetadata> PostVideoMetadataAsync(VideoMetadata videoMetadata);
        ValueTask<List<VideoMetadata>> GetAllVideoMetadatasAsync();
    }
}
