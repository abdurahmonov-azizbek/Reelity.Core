// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Reelity.Core.Api.Models.VideoMetadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using System;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            //given
            DateTimeOffset someDateTime = GetRandomDateTimeOffset();
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata(someDateTime);
            VideoMetadata someVideoMetadata = randomVideoMetadata;
            Guid videoMetadataId = someVideoMetadata.Id;
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
                broker.SelectVideoMetadataByIdAsync(videoMetadataId))
                    .ThrowsAsync(sqlException);

            //when
            ValueTask<VideoMetadata> modifyVideoMetadataTask =
                this.videoMetadataService.ModifyVideoMetadataAsync(someVideoMetadata);

            VideoMetadataDependencyException actualVideoMetadataDependencyException =
                await Assert.ThrowsAsync<VideoMetadataDependencyException>(
                    modifyVideoMetadataTask.AsTask);

            //then
            actualVideoMetadataDependencyException.Should().BeEquivalentTo(
                    expectedVideoMetadataDependencyException);

            this.storageBrokerMock.Verify(broker =>
                 broker.SelectVideoMetadataByIdAsync(videoMetadataId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedVideoMetadataDependencyException))));

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given 
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata();
            VideoMetadata someVideoMetadata = randomVideoMetadata;
            Guid videoMetadataId = someVideoMetadata.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedVideoMetadataStorageException =
                new FailedVideoMetadataStorageException(
                    message: "Failed Video metadata error occured, contact support.",
                    innerException: databaseUpdateException);

            var expectedVideoMetadataDependencyException =
                new VideoMetadataDependencyException(
                    message: "Video metadata dependency error occured, fix the errors and try again.",
                    innerException: failedVideoMetadataStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(videoMetadataId))
                    .ThrowsAsync(databaseUpdateException);

            // when 
            ValueTask<VideoMetadata> modifyVideoMetadataTask =
                this.videoMetadataService.ModifyVideoMetadataAsync(someVideoMetadata);

            VideoMetadataDependencyException actualVideoMetadataDependencyException =
                await Assert.ThrowsAsync<VideoMetadataDependencyException>(modifyVideoMetadataTask.AsTask);

            // then
            actualVideoMetadataDependencyException.Should()
                .BeEquivalentTo(expectedVideoMetadataDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(videoMetadataId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedVideoMetadataDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyExceptionOccursAndLogItAsync()
        {
            // given 
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata();
            VideoMetadata someVideoMetadata = randomVideoMetadata;
            Guid videoMetadataId = someVideoMetadata.Id;
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedVideoMetadataException =
                new LockedVideoMetadataException(
                    message: "Video metadata is locked, try again later.",
                    innerException: databaseUpdateConcurrencyException);

            var expectedVideoMetadataDependencyException =
                new VideoMetadataDependencyException(
                    message: "Video metadata dependency error occured, fix the errors and try again.",
                    innerException: lockedVideoMetadataException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectVideoMetadataByIdAsync(videoMetadataId))
                        .ThrowsAsync(databaseUpdateConcurrencyException);

            // when 
            ValueTask<VideoMetadata> modifyVideoMetadataTask =
                this.videoMetadataService.ModifyVideoMetadataAsync(someVideoMetadata);

            VideoMetadataDependencyException actualVideoMetadataDependencyException =
                await Assert.ThrowsAsync<VideoMetadataDependencyException>(modifyVideoMetadataTask.AsTask);

            // then
            actualVideoMetadataDependencyException.Should()
                .BeEquivalentTo(expectedVideoMetadataDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(videoMetadataId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedVideoMetadataDependencyException))));

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
        {
            // given 
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata();
            VideoMetadata someVideoMetadata = randomVideoMetadata;
            Guid videoMetadataId = someVideoMetadata.Id;
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
                    broker.SelectVideoMetadataByIdAsync(videoMetadataId)).ThrowsAsync(serviceException);

            // when 
            ValueTask<VideoMetadata> modifyVideoMetadataTask =
                this.videoMetadataService.ModifyVideoMetadataAsync(someVideoMetadata);

            VideoMetadataServiceException actualVideoMetadataServiceException =
                await Assert.ThrowsAsync<VideoMetadataServiceException>(modifyVideoMetadataTask.AsTask);

            // then
            actualVideoMetadataServiceException.Should()
                .BeEquivalentTo(expectedVideoMetadataServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(videoMetadataId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedVideoMetadataServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
