// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
using Force.DeepCloner;
using Moq;
using Reelity.Core.Api.Models.Metadatas;
using System.Linq;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllVideoMetadatas()
        {
            // given
            IQueryable<VideoMetadata> randomLanguages = CreateRandomLanguages();
            IQueryable<VideoMetadata> storageLanguages = randomLanguages;
            IQueryable<VideoMetadata> expectedLanguages = storageLanguages.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllVideoMetadatas())
                    .Returns(storageLanguages);

            // when
            IQueryable<VideoMetadata> actualVideoMetadatas =
                this.videoMetadataService.RetrieveAllLanguages();

            // then
            actualVideoMetadatas.Should().BeEquivalentTo(expectedLanguages);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllVideoMetadatas(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
