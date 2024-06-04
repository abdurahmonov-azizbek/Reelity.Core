// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Api.Models.Metadatas;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Reelity.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<VideoMetadata> VideoMetadatas { get; set; }
        
        public async ValueTask<VideoMetadata> InsertVideoMetadataAsync(VideoMetadata videoMetadata) =>
            await InsertAsync(videoMetadata);

        public IQueryable<VideoMetadata> SelectAllVideoMetadatas() =>
            SelectAll<VideoMetadata>();

        public async ValueTask<VideoMetadata> SelectVideoMetadataByIdAsync(Guid videoMetadataId) =>
           await SelectAsync<VideoMetadata>(videoMetadataId);

        public async ValueTask<VideoMetadata> DeleteVideoMetadataAsync(VideoMetadata videoMetadata) =>
            await DeleteAsync(videoMetadata);
    }
}
