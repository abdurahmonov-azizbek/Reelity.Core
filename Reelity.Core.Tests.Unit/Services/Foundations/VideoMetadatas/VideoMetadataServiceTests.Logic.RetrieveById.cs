// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Reelity.Core.Api.Models.Metadatas;
using System;
using System.Threading.Tasks;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveVideoMetadataByIdAsync()
        {
            //given
            Guid randomVideoMetadataId = Guid.NewGuid();
            Guid inputVideoMetadataId = randomVideoMetadataId;
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata();
            VideoMetadata persistedVideoMetadata = randomVideoMetadata;
            VideoMetadata expectedVideoMetadata = persistedVideoMetadata.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(inputVideoMetadataId))
                    .ReturnsAsync(persistedVideoMetadata);

            //when
            VideoMetadata actualVideoMetadata = await this.videoMetadataService
                .RetrieveVideoMetadataByIdAsync(inputVideoMetadataId);

            //then
            actualVideoMetadata.Should().BeEquivalentTo(expectedVideoMetadata);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(inputVideoMetadataId), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
