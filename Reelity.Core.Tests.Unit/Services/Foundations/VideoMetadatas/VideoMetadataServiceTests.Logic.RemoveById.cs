// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Reelity.Core.Api.Models.VideoMetadatas;
using System;
using System.Threading.Tasks;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShuouldRemoveVideoMetadataByIdAsync()
        {
            // given
            Guid randomVideoMetadataId = Guid.NewGuid();
            Guid inputVideoMetadataId = randomVideoMetadataId;
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata();
            VideoMetadata storageVideoMetadata = randomVideoMetadata;
            VideoMetadata expectedInputVideoMetadata = storageVideoMetadata;
            VideoMetadata deletedVideoMetadata = expectedInputVideoMetadata;
            VideoMetadata expectedVideoMetadata = deletedVideoMetadata.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(inputVideoMetadataId))
                    .ReturnsAsync(storageVideoMetadata);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteVideoMetadataAsync(expectedInputVideoMetadata))
                    .ReturnsAsync(deletedVideoMetadata);

            // when
            VideoMetadata actualVideoMetadata = await this.videoMetadataService
                .RemoveVideoMetadataByIdAsync(inputVideoMetadataId);

            // then
            actualVideoMetadata.Should().BeEquivalentTo(expectedVideoMetadata);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(inputVideoMetadataId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteVideoMetadataAsync(expectedInputVideoMetadata), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
