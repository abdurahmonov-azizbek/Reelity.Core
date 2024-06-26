﻿// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.Data.SqlClient;
using Moq;
using Reelity.Core.Api.Brokers.DateTimes;
using Reelity.Core.Api.Brokers.Loggings;
using Reelity.Core.Api.Brokers.Storages;
using Reelity.Core.Api.Models.VideoMetadatas;
using Reelity.Core.Api.Services.VideoMetadatas;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Tynamix.ObjectFiller;
using Xeptions;

namespace Reelity.Core.Tests.Unit.Services.Foundations.VideoMetadatas
{
    public partial class VideoMetadataServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly IVideoMetadataService videoMetadataService;

        public VideoMetadataServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();

            this.videoMetadataService = new VideoMetadataService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object);
        }

        private IQueryable<VideoMetadata> CreateRandomVideoMetadatas()
        {
            return CreateRandomVideoMetadataFiller(date: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber()).AsQueryable();
        }

        private static VideoMetadata CreateRandomVideoMetadata() =>
           CreateRandomVideoMetadataFiller(date: GetRandomDateTimeOffset()).Create();

        private static VideoMetadata CreateRandomVideoMetadata(DateTimeOffset date) =>
            CreateRandomVideoMetadataFiller(date).Create();

        private static SqlException CreateSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        public static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Filler<VideoMetadata> CreateRandomVideoMetadataFiller(DateTimeOffset date)
        {
            var filler = new Filler<VideoMetadata>();

            filler.Setup()
               .OnType<DateTimeOffset>().Use(date);

            return filler;
        }

        private VideoMetadata CreateRandomModifyVideoMetadata(DateTimeOffset dates)
        {
            int randomdaysInPast = GetRandomNegativeNumber();
            VideoMetadata randomVideoMetadata = CreateRandomVideoMetadata(dates);

            randomVideoMetadata.CreatedDate = randomVideoMetadata.CreatedDate.AddDays(randomdaysInPast);

            return randomVideoMetadata;
        }

        private static Expression<Func<Exception, bool>> SameExceptionAs(Exception expectedException)
        {
            return actualException =>
                actualException.Message == expectedException.Message
                && actualException.InnerException.Message == expectedException.InnerException.Message;
        }

        private static Expression<Func<Exception, bool>> SameValidationExceptionAs(Exception expectedException)
        {
            return actualException =>
                actualException.Message == expectedException.Message
                && actualException.InnerException.Message == expectedException.InnerException.Message
                && (actualException.InnerException as Xeption).DataEquals(expectedException.InnerException.Data);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static SqlException GetSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNegativeNumber() =>
         -1 * new IntRange(min: 2, max: 9).GetValue();
    }
}
