// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Moq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading.Tasks;
using Reelity.Core.Api.Models.VideoMetadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using FluentAssertions;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfVideoMetadataIsNullAndLogItAsync()
        {
            // given
            VideoMetadata nullVideoMetadata = null;
            var nullVideoMetadataException = new NullVideoMetadataException("VideoMetadata is null.");

            var expectedVideoMetadataValidationException =
                new VideoMetadataValidationException(
                    message: "Video Metadata Validation Exception occured the errors and try again.",
                    innerException: nullVideoMetadataException);


            // when
            ValueTask<VideoMetadata> modifyCompanyTask = 
                this.videoMetadataService.ModifyVideoMetadataAsync(nullVideoMetadata);

            VideoMetadataValidationException actualVideoMetadataValidationException =
                await Assert.ThrowsAsync<VideoMetadataValidationException>(modifyCompanyTask.AsTask);

            // then
            actualVideoMetadataValidationException.Should()
                .BeEquivalentTo(expectedVideoMetadataValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedVideoMetadataValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
