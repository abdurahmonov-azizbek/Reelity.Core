// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Reelity.Core.Api.Models.VideoMetadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using System;

namespace Reelity.Core.Api.Services.VideoMetadatas
{
    public partial class VideoMetadataService
    {
        private void ValidateVideoMetadataOnAdd(VideoMetadata videoMetadata)
        {
            ValidateVideoMetadataNotNull(videoMetadata);

            Validate(
                (Rule: IsInvalid(videoMetadata.Id), Parameter: nameof(VideoMetadata.Id)),
                (Rule: IsInvalid(videoMetadata.Title), Parameter: nameof(VideoMetadata.Title)),
                (Rule: IsInvalid(videoMetadata.BlobPath), Parameter: nameof(VideoMetadata.BlobPath)),
                (Rule: IsInvalid(videoMetadata.CreatedDate), Parameter: nameof(VideoMetadata.CreatedDate)),
                (Rule: IsInvalid(videoMetadata.UpdatedDate), Parameter: nameof(VideoMetadata.UpdatedDate)),

                (Rule: IsNotSame(
                        firstDate: videoMetadata.UpdatedDate,
                        secondDate: videoMetadata.CreatedDate,
                        secondDateName: nameof(videoMetadata.CreatedDate)),
                   Parameter: nameof(videoMetadata.UpdatedDate)));
        }

        private void ValidateVideoMetadataOnModify(VideoMetadata videoMetadata)
        {
            ValidateVideoMetadataNotNull(videoMetadata);

            Validate(
                (Rule: IsInvalid(videoMetadata.Id), Parameter: nameof(VideoMetadata.Id)),
                (Rule: IsInvalid(videoMetadata.Title), Parameter: nameof(VideoMetadata.Title)),
                (Rule: IsInvalid(videoMetadata.BlobPath), Parameter: nameof(VideoMetadata.BlobPath)),
                (Rule: IsInvalid(videoMetadata.CreatedDate), Parameter: nameof(VideoMetadata.CreatedDate)),
                (Rule: IsInvalid(videoMetadata.UpdatedDate), Parameter: nameof(VideoMetadata.UpdatedDate)),

               (Rule: IsNotSame(
                        firstDate: videoMetadata.UpdatedDate,
                        secondDate: videoMetadata.CreatedDate,
                        secondDateName: nameof(videoMetadata.CreatedDate)),
                   Parameter: nameof(videoMetadata.UpdatedDate)));
        }

        private static void ValidateStorageVideoMetadata(VideoMetadata mayVideoMetadata, Guid videoMetadataId)
        {
            if (mayVideoMetadata is null)
            {
                throw new NotFoundVideoMetadataException(
                    $"Couldn't find language with id:{videoMetadataId}",
                    videoMetadataId);
            }
        }

        private void ValidateVideoMetadataId(Guid videoMetadataId) =>
             Validate((Rule: IsInvalid(videoMetadataId), Parameter: nameof(VideoMetadata.Id)));

        private void ValidateVideoMetadataNotNull(VideoMetadata videoMetadata)
        {
            if (videoMetadata is null)
            {
                throw new NullVideoMetadataException(message: "VideoMetadata is null.");
            }
        }

        private static dynamic IsInvalid(Guid Id) => new
        {
            Condition = Id == Guid.Empty,
            Message = "Id is required."
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required."
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default(DateTimeOffset),
            Message = "Date is required."
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidVideoMetadataException = new InvalidVideoMetadataException(
                message: "Video Metadata is invalid.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidVideoMetadataException.UpsertDataList(parameter, rule.Message);
                }
            }

            invalidVideoMetadataException.ThrowIfContainsErrors();
        }
    }
}
