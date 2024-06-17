﻿// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Api.Brokers.DateTimes;
using Reelity.Core.Api.Brokers.Loggings;
using Reelity.Core.Api.Brokers.Storages;
using Reelity.Core.Api.Models.VideoMetadatas;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public IQueryable<VideoMetadata> RetrieveAllVideoMetadatas() =>
            TryCatch(() => this.storageBroker.SelectAllVideoMetadatas());

        public ValueTask<VideoMetadata> RetrieveVideoMetadataByIdAsync(Guid videoMetadataId) =>
           TryCatch(async () =>
           {
               ValidateVideoMetadataId(videoMetadataId);

               VideoMetadata maybeVideoMetadata =
                   await storageBroker.SelectVideoMetadataByIdAsync(videoMetadataId);

               ValidateStorageVideoMetadataExists(maybeVideoMetadata, videoMetadataId);

               return maybeVideoMetadata;
           });
    }
}
