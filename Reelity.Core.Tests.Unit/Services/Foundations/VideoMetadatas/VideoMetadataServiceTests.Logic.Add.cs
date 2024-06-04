// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Reelity.Core.Api.Models.Metadatas;
using System.Threading.Tasks;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldAddVideoMetadataAsync()
        {
            //given
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata();
            VideoMetadata inputVideoMetadata = randomVideoMetadata;
            VideoMetadata persistedVideoMetadata = inputVideoMetadata;
            VideoMetadata expectedVideoMetadata = persistedVideoMetadata.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertVideoMetadataAsync(inputVideoMetadata))
                    .ReturnsAsync(persistedVideoMetadata);

            //when
            VideoMetadata actualVideoMetadata = await this.videoMetadataService
                .AddVideoMetadataAsync(inputVideoMetadata);

            //then  
            actualVideoMetadata.Should().BeEquivalentTo(
                expectedVideoMetadata);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertVideoMetadataAsync(inputVideoMetadata),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
