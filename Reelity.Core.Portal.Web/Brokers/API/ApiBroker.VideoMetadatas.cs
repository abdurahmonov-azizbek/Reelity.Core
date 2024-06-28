// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Portal.Web.Models.VideoMetadatas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reelity.Core.Portal.Web.Brokers.API
{
    public partial class ApiBroker
    {
        private const string VideoMetadatasRelativeUrl = "api/videometadatas";

        public async ValueTask<VideoMetadata> PostVideoMetadataAsync(VideoMetadata videoMetadata) =>
            await this.PostAsync(VideoMetadatasRelativeUrl, videoMetadata);

        public async ValueTask<List<VideoMetadata>> GetAllVideoMetadatasAsync() =>
            await this.GetAsync<List<VideoMetadata>>(VideoMetadatasRelativeUrl);
    }
}
