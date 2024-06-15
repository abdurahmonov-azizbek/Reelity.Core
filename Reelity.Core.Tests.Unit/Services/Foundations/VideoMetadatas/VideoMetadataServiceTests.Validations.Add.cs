// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Moq;
using Reelity.Core.Api.Models.VideoMetadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using System;
using System.Threading.Tasks;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfIsNullAndLogError()
        {
            //given
            VideoMetadata nullVideoMetadata = null;
            var nullVideoMetadataException = new NullVideoMetadataException("VideoMetadata is null.");

            var expectedVideoMetadataValidationException =
                new VideoMetadataValidationException(
                    "Video Metadata Validation Exception occured, fix the errors and try again.",
                        nullVideoMetadataException);

            //when
            ValueTask<VideoMetadata> addVideoMetadata =
                this.videoMetadataService.AddVideoMetadataAsync(nullVideoMetadata);

            VideoMetadataValidationException actualVideoMetadataValidationException =
                await Assert.ThrowsAsync<VideoMetadataValidationException>(addVideoMetadata.AsTask);

            //then
            actualVideoMetadataValidationException.Should()
                .BeEquivalentTo(expectedVideoMetadataValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertVideoMetadataAsync(It.IsAny<VideoMetadata>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfVideoMetadataIsInvalidDataAndLogItAsync(string invalidData)
        {
            //given
            var invalidVideoMetadata = new VideoMetadata()
            {
                Title = invalidData
            };

            var invalidVideoMetadataException = new InvalidVideoMetadataException(
                message: "Video Metadata is invalid.");

            invalidVideoMetadataException.AddData(key: nameof(VideoMetadata.Id),
                values: "Id is required.");

            invalidVideoMetadataException.AddData(key: nameof(VideoMetadata.Title),
                values: "Text is required.");

            invalidVideoMetadataException.AddData(key: nameof(VideoMetadata.BlobPath),
                values: "Text is required.");

            invalidVideoMetadataException.AddData(key: nameof(VideoMetadata.CreatedDate),
                values: "Date is required.");

            invalidVideoMetadataException.AddData(key: nameof(VideoMetadata.UpdatedDate),
                values: "Date is required.");

            var expectedVideoMetadataValidationException =
                new VideoMetadataValidationException(
                    message: "Video Metadata Validation Exception occured, fix the errors and try again.",
                    innerException: invalidVideoMetadataException);

            //when
            ValueTask<VideoMetadata> addVideoMetadataTask =
                this.videoMetadataService.AddVideoMetadataAsync(invalidVideoMetadata);

            VideoMetadataValidationException actualVideoMetadataValidationException =
                await Assert.ThrowsAsync<VideoMetadataValidationException>(addVideoMetadataTask.AsTask);


            //then
            actualVideoMetadataValidationException.Should().BeEquivalentTo(expectedVideoMetadataValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedVideoMetadataValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertVideoMetadataAsync(It.IsAny<VideoMetadata>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomNumber = GetRandomNumber();
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata(randomDateTime);
            VideoMetadata invalidVideoMetadata = randomVideoMetadata;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTime);

            invalidVideoMetadata.UpdatedDate =
                invalidVideoMetadata.CreatedDate.AddDays(randomNumber);

            var invalidVideoMetadataException =
                new InvalidVideoMetadataException(
                    message: "Video Metadata is invalid.");

            invalidVideoMetadataException.AddData(
                key: nameof(VideoMetadata.UpdatedDate),
                values: $"Date is not the same as {nameof(VideoMetadata.CreatedDate)}");

            var expectedVideoMetadataValidationException =
                new VideoMetadataValidationException(
                    message: "Video Metadata Validation Exception occured, fix the errors and try again.",
                    innerException: invalidVideoMetadataException);

            // when
            ValueTask<VideoMetadata> addVideoMetadataTask =
                this.videoMetadataService.AddVideoMetadataAsync(invalidVideoMetadata);

            var actualVideoMetadataValidationException =
                await Assert.ThrowsAsync<VideoMetadataValidationException>(
                    addVideoMetadataTask.AsTask);

            // then
            actualVideoMetadataValidationException.Should().BeEquivalentTo(
                expectedVideoMetadataValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameValidationExceptionAs(
                    expectedVideoMetadataValidationException))),
                        Times.Once);


            this.storageBrokerMock.Verify(broker =>
                broker.InsertVideoMetadataAsync(It.IsAny<VideoMetadata>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
