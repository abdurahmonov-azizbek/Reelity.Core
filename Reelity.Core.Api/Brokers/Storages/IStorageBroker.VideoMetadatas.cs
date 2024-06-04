// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.Extensions.Hosting;
using Reelity.Core.Api.Models.Metadatas;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Reelity.Core.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<VideoMetadata> InsertVideoMetadataAsync(VideoMetadata videoMetadata);
        IQueryable<VideoMetadata> SelectAllVideoMetadatas();
        ValueTask<VideoMetadata> SelectVideoMetadataByIdAsync(Guid videoMetadataId);
        ValueTask<VideoMetadata> UpdateVideoMetadataAsync(VideoMetadata videoMetadata);
        ValueTask<VideoMetadata> DeleteVideoMetadataAsync(VideoMetadata videoMetadata);
    }
}