// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Moq;
using Reelity.Core.Portal.Web.Models.VideoMetadatas;
using System.Threading.Tasks;
using Xunit;

namespace Reelity.Core.Portal.Web.Tests.Unit.Services.Foundations
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public async Task ShouldRegisterVideoMetadataAsync()
        {
            // given
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata();
            VideoMetadata inputVideoMetadata = randomVideoMetadata;
            VideoMetadata retrievedVideoMetadata = inputVideoMetadata;
            VideoMetadata expectedVideoMetadata = retrievedVideoMetadata;

            this.apiBrokerMock.Setup(broker =>
                broker.PostVideoMetadataAsync(inputVideoMetadata))
                    .ReturnsAsync(retrievedVideoMetadata);

            // when
            VideoMetadata actualVideoMetadata =
                await this.videoMetadataService
                    .AddVideoMetadataAsync(inputVideoMetadata);

            // then
            actualVideoMetadata.Should().BeEquivalentTo(expectedVideoMetadata);

            this.apiBrokerMock.Verify(broker =>
                broker.PostVideoMetadataAsync(inputVideoMetadata),
                    Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
