﻿// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Reelity.Core.Api.Brokers.DateTimes;
using Reelity.Core.Api.Brokers.Loggings;
using Reelity.Core.Api.Brokers.Storages;
using Reelity.Core.Api.Models.VideoMetadatas;
using System;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Reelity.Core.Api.Services.VideoMetadatas
{
    public partial class VideoMetadataService : IVideoMetadataService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public VideoMetadataService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<VideoMetadata> AddVideoMetadataAsync(VideoMetadata videoMetadata) =>
            TryCatch(async () =>
                {
                    ValidateVideoMetadataOnAdd(videoMetadata);

                    return await this.storageBroker.InsertVideoMetadataAsync(videoMetadata);
                });

        public ValueTask<VideoMetadata> RemoveVideoMetadataByIdAsync(Guid videoMetadataId) =>
            TryCatch(async () =>
           {
                ValidateVideoMetadataId(videoMetadataId);

                VideoMetadata maybeVideoMetadata =
                await this.storageBroker.SelectVideoMetadataByIdAsync(videoMetadataId);

                ValidateStorageVideoMetadataExists(maybeVideoMetadata, videoMetadataId);

                return await this.storageBroker.DeleteVideoMetadataAsync(maybeVideoMetadata);
        });
        
        public ValueTask<VideoMetadata> DeleteVideoMetadataAsync(Guid videoMetadataId)
        {
            throw new NotImplementedException();
        }

        public ValueTask<VideoMetadata> ModifyVideoMetadataAsync(VideoMetadata videoMetadata) =>
            TryCatch(async () =>
            {
                ValidateVideoMetadataOnModify(videoMetadata);

                var maybeVideoMetadata =
                    await this.storageBroker.SelectVideoMetadataByIdAsync(videoMetadata.Id);

                ValidateAgainstStorageOnModify(
                    inputVideoMetadata: videoMetadata,
                    storageVideoMetadata: maybeVideoMetadata);

                return await this.storageBroker.UpdateVideoMetadataAsync(videoMetadata);
            });

        public IQueryable<VideoMetadata> RetrieveAllVideoMetadatas() =>
            TryCatch(() => this.storageBroker.SelectAllVideoMetadatas());

        public ValueTask<VideoMetadata> RetrieveVideoMetadataByIdAsync(Guid videoMetadataId) =>
            TryCatch(async () =>
            {
                ValidateVideoMetadataId(videoMetadataId);

                VideoMetadata maybeVideoMetadata =
                    await this.storageBroker.SelectVideoMetadataByIdAsync(videoMetadataId);

                ValidateStorageVideoMetadata(maybeVideoMetadata, videoMetadataId);

                return maybeVideoMetadata;
            });
    }
}
