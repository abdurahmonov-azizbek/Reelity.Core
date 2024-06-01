// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Api.Models.Metadatas;
using System;
using System.Threading.Tasks;

namespace Reelity.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public async ValueTask<VideoMetadata> SelectVideoMetadataByIdAsync(Guid videoMetadataId) =>
           await SelectAsync<VideoMetadata>(videoMetadataId);
    }
}
