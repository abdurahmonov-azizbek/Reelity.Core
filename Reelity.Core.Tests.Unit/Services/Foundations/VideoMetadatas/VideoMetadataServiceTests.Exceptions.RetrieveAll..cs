// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using System;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageException =
                new FailedVideoMetadataStorageException(
                    message: "Failed Video metadata storage exception occurred, contact support",
                    innerException: sqlException);

            var expectedVideoMetadataDependencyException =
                new VideoMetadataDependencyException(
                    message: "Video metadata dependency exception occurred, contact support",
                    innerException: failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllVideoMetadatas())
                    .Throws(sqlException);

            // when
            Action retrieveAllVideoMetadatasAction = () =>
                this.videoMetadataService.RetrieveAllVideoMetadatas();

            VideoMetadataDependencyException actualVideoMetadataDependencyException =
                Assert.Throws<VideoMetadataDependencyException>(
                    retrieveAllVideoMetadatasAction);

            // then
            actualVideoMetadataDependencyException.Should().BeEquivalentTo(
                expectedVideoMetadataDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllVideoMetadatas(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedVideoMetadataDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
