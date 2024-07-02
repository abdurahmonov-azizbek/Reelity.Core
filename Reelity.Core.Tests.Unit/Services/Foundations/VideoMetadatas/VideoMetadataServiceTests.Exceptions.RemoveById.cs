﻿// -------------------------------------------------------
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

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            //given
            Guid someVideoMetadataId = Guid.NewGuid();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedVideoMetadataException =
                new LockedVideoMetadataException(
                    message: "Video metadata is locked, try again later.",
                    innerException: databaseUpdateConcurrencyException);

            var expectedVideoMetadataDependencyValidationException =
                new VideoMetadataDependencyValidationException(
                    message: "Video metadata dependency error occured, fix the errors and try again.",
                    innerException: lockedVideoMetadataException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            //when
            ValueTask<VideoMetadata> removeVideoMetadataByIdTask =
                this.videoMetadataService.RemoveVideoMetadataByIdAsync(someVideoMetadataId);

            VideoMetadataDependencyValidationException actualVideoMetadataDependencyValidationException =
                 await Assert.ThrowsAsync<VideoMetadataDependencyValidationException>(
                    removeVideoMetadataByIdTask.AsTask);

            //then
            actualVideoMetadataDependencyValidationException.Should().BeEquivalentTo(
                expectedVideoMetadataDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>()), Times.Once);


            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedVideoMetadataDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteVideoMetadataAsync(It.IsAny<VideoMetadata>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnDeleteWhenSqlExceptionOccursAndLogItAsync()
        {
            //given
            Guid someVideoMetadataId = Guid.NewGuid();
            SqlException sqlException = CreateSqlException();

            var failedVideoMetadataStorageException =
                new FailedVideoMetadataStorageException(
                    "Failed Video metadata error occured, contact support.",
                    sqlException);

            var expectedVideoMetadataDependencyException =
                new VideoMetadataDependencyException(
                    message: "Video metadata dependency error occured, fix the errors and try again.",
                    innerException: failedVideoMetadataStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(someVideoMetadataId))
                    .Throws(sqlException);

            //when
            ValueTask<VideoMetadata> deleteVideoMetadataTask =
                this.videoMetadataService.RemoveVideoMetadataByIdAsync(someVideoMetadataId);

            VideoMetadataDependencyException actualVideoMetadataDependencyException =
                    await Assert.ThrowsAsync<VideoMetadataDependencyException>(deleteVideoMetadataTask.AsTask);

            // then
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
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someVideoMetadataId = Guid.NewGuid();
            var seviceException = new Exception();

            var failedVideoMetadataServiceException =
               new FailedVideoMetadataServiceException(
                   message: "Failed Video metadata service error occured, please contact support",
                   innerException: seviceException);

            var expectedVideoMetadataServiceException =
                new VideoMetadataServiceException(
                    message: "Video metadata service error occurred, contact support.",
                    innerException: failedVideoMetadataServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(seviceException);

            // when
            ValueTask<VideoMetadata> removeVideoMetadataByIdTask =
                this.videoMetadataService.RemoveVideoMetadataByIdAsync(someVideoMetadataId);

            VideoMetadataServiceException actualVideoMetadataServiceException =
                await Assert.ThrowsAsync<VideoMetadataServiceException>(
                    removeVideoMetadataByIdTask.AsTask);

            // then
            actualVideoMetadataServiceException.Should().BeEquivalentTo(
                expectedVideoMetadataServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectVideoMetadataByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedVideoMetadataServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
