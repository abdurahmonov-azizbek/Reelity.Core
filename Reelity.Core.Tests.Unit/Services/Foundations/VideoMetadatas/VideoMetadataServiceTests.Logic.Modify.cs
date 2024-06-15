// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Reelity.Core.Api.Models.VideoMetadatas;
using System;
using System.Threading.Tasks;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldModifyVideoMetadataAsync()
        {
            //given
            DateTimeOffset randomDate = GetRandomDateTimeOffset();
            VideoMetadata randomVideoMetadata = CreateRandomModifyVideoMetadata(randomDate);
            VideoMetadata inputVideoMetadata = randomVideoMetadata;
            VideoMetadata storageVideoMetadata = inputVideoMetadata.DeepClone();
            storageVideoMetadata.UpdatedDate = randomVideoMetadata.CreatedDate;
            VideoMetadata updatedVideoMetadata = inputVideoMetadata;
            VideoMetadata expectedVideoMetadata = updatedVideoMetadata.DeepClone();
            Guid videoMetadataId = inputVideoMetadata.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(videoMetadataId))
                    .ReturnsAsync(storageVideoMetadata);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateVideoMetadataAsync(inputVideoMetadata))
                    .ReturnsAsync(updatedVideoMetadata);

            //when
            VideoMetadata actualVideoMetadata =
                await this.videoMetadataService.ModifyVideoMetadataAsync(inputVideoMetadata);

            //then
            actualVideoMetadata.Should().BeEquivalentTo(expectedVideoMetadata);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(videoMetadataId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateVideoMetadataAsync(inputVideoMetadata), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
