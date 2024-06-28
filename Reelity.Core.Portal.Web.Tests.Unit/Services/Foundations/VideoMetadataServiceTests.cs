// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Moq;
using Reelity.Core.Portal.Web.Brokers.API;
using Reelity.Core.Portal.Web.Brokers.Loggings;
using Reelity.Core.Portal.Web.Services.Foundations.VideoMetadatas;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Tynamix.ObjectFiller;
using Xeptions;
using Reelity.Core.Portal.Web.Models.VideoMetadatas;
using System.Linq;

namespace Reelity.Core.Portal.Web.Tests.Unit.Services.Foundations
{
    public partial class VideoMetadataServiceTests
    {
        private readonly Mock<IApiBroker> apiBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IVideoMetadataService videoMetadataService;

        public VideoMetadataServiceTests()
        {
            this.apiBrokerMock = new Mock<IApiBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.videoMetadataService = new VideoMetadataService(
                apiBroker: this.apiBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static VideoMetadata CreateRandomVideoMetadata() =>
            CreateVideoMetadataFiller().Create();

        private static List<VideoMetadata> CreateRandomVideoMetadatas() =>
            CreateVideoMetadataFiller().Create(count: GetRandomNumber()).ToList();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException)
        {
            return actualException => actualException.Message == expectedException.Message
                && actualException.InnerException.Message == expectedException.InnerException.Message
                && (actualException.InnerException as Xeption).DataEquals(expectedException.InnerException.Data);
        }

        private static string GetRandomString() => 
            new MnemonicString().GetValue();

        private static int GetRandomNumber() => 
            new IntRange(min: 2, max: 10).GetValue();

        private static Filler<VideoMetadata> CreateVideoMetadataFiller()
        {
            var filler = new Filler<VideoMetadata>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow);

            return filler;
        }
    }
}
