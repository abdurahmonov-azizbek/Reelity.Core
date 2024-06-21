// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Moq;
using System.Threading.Tasks;
using System;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using Reelity.Core.Api.Models.VideoMetadatas;
using FluentAssertions;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            //given
            var invalideVideoMetadataId = Guid.Empty;
            var invalidVideoMetadataException = new InvalidVideoMetadataException(
                message: "Video Metadata is invalid.");

            invalidVideoMetadataException.AddData(
                key: nameof(VideoMetadata.Id),
                values: "Id is required.");

            var expectedVideoMetadataValidationException =
                new VideoMetadataValidationException(
                    message: "Video Metadata Validation Exception occured, fix the errors and try again.",
                    innerException: invalidVideoMetadataException);

            //when
            ValueTask<VideoMetadata> retrieveVideoMetadataById =
                this.videoMetadataService.RetrieveVideoMetadataByIdAsync(invalideVideoMetadataId);

            VideoMetadataValidationException actualVideoMetadataValidationExcaption =
                await Assert.ThrowsAsync<VideoMetadataValidationException>(
                    retrieveVideoMetadataById.AsTask);
            //then
            actualVideoMetadataValidationExcaption.Should().
                BeEquivalentTo(expectedVideoMetadataValidationException);

            this.loggingBrokerMock.Verify(broker =>
              broker.LogError(It.Is(SameExceptionAs(
                  expectedVideoMetadataValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfVideoMetadataIsNotFoundAndLogItAsync()
        {
            //given
            Guid someVideoMetadataId = Guid.NewGuid();
            VideoMetadata noVideoMetadata = null;

            var notFoundVideoMetadataValidationException =
                new NotFoundVideoMetadataException(
                    $"Couldn't find video metadata with id {someVideoMetadataId}");

            var expectedVideoMetadataValidationException =
                new VideoMetadataValidationException(
                    message: "Video Metadata Validation Exception occured, fix the errors and try again.",
                    innerException: notFoundVideoMetadataValidationException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>())).ReturnsAsync(noVideoMetadata);

            //when
            ValueTask<VideoMetadata> retrieveByIdVideoMetadataTask =
                this.videoMetadataService.RetrieveVideoMetadataByIdAsync(someVideoMetadataId);

            VideoMetadataValidationException actualVideoMetadataValidationExcaption =
                await Assert.ThrowsAsync<VideoMetadataValidationException>(
                    retrieveByIdVideoMetadataTask.AsTask);

            //then
            actualVideoMetadataValidationExcaption.Should().
                BeEquivalentTo(expectedVideoMetadataValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedVideoMetadataValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
