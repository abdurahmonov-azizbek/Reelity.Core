// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Portal.Web.Brokers.API;
using Reelity.Core.Portal.Web.Brokers.Loggings;
using Reelity.Core.Portal.Web.Models.VideoMetadatas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reelity.Core.Portal.Web.Services.Foundations.VideoMetadatas
{
    public class VideoMetadataService : IVideoMetadataService
    {
        private readonly IApiBroker apiBroker;
        private readonly ILoggingBroker loggingBroker;

        public VideoMetadataService(
            IApiBroker apiBroker,
            ILoggingBroker loggingBroker)
        {
            this.apiBroker = apiBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<VideoMetadata> AddVideoMetadataAsync(VideoMetadata videoMetadata)
        {
            throw new NotImplementedException();
        }

        public ValueTask<List<VideoMetadata>> RetrieveAllVideoMetadatasAsync()
        {
            throw new NotImplementedException();
        }
    }
}
