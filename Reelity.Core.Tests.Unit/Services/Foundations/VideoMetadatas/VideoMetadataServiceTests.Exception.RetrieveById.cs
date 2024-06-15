// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.Data.SqlClient;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdAsyncIfSqlErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedVideoMetadataStorageException =
                new FailedVideoMetadataStorageException(
                    message: "Failed Video metadata error occured, contact support.",
                    innerException: sqlException);

            var expectedVideoMetadataDependencyException =
                new VideoMetadataDependencyException(
                    message: "Video metadata dependency error occured, fix the errors and try again.",
                    innerException: failedVideoMetadataStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>())).ThrowsAsync(sqlException);

            //when
            ValueTask<VideoMetadata> retrieveVideoMetadataByIdTask =
                this.videoMetadataService.RetrieveVideoMetadataByIdAsync(someId);

            VideoMetadataDependencyException actualVideoMetadataDependencyException =
                await Assert.ThrowsAsync<VideoMetadataDependencyException>(retrieveVideoMetadataByIdTask.AsTask);

            //then
            actualVideoMetadataDependencyException.Should().BeEquivalentTo(expectedVideoMetadataDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedVideoMetadataDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            //given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedVideoMetadataServiceException =
                new FailedVideoMetadataServiceException(
                    message: "Failed Video metadata service error occured, please contact support",
                    innerException: serviceException);

            var expectedVideoMetadataServiceException =
                new VideoMetadataServiceException(
                    message: "Video metadata service error occurred, contact support.",
                    innerException: failedVideoMetadataServiceException);

            this.storageBrokerMock.Setup(broker =>
               broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>())).ThrowsAsync(serviceException);

            //when
            ValueTask<VideoMetadata> retrieveVideoMetadataById =
                this.videoMetadataService.RetrieveVideoMetadataByIdAsync(someId);

            VideoMetadataServiceException actualVideoMetadataServiceException =
                await Assert.ThrowsAsync<VideoMetadataServiceException>(retrieveVideoMetadataById.AsTask);

            // then
            actualVideoMetadataServiceException.Should().BeEquivalentTo(expectedVideoMetadataServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedVideoMetadataServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
