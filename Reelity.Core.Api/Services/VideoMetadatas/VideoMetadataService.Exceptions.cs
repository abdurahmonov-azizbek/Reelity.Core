// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.Data.SqlClient;
using Reelity.Core.Api.Models.Metadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using System;
using System.Threading.Tasks;
using Xeptions;

namespace Reelity.Core.Api.Services.VideoMetadatas
{
    public partial class VideoMetadataService
    {
        private delegate ValueTask<VideoMetadata> ReturningVideoMetadataFunction();

        private async ValueTask<VideoMetadata> TryCatch(ReturningVideoMetadataFunction returningVideoMetadataFunction)
        {
            try
            {
                return await returningVideoMetadataFunction();
            }
            catch(NullVideoMetadataException nullVideoMetadataException)
            {
                throw CreateAndLogValidationException(nullVideoMetadataException);
            }
            catch (SqlException sqlException)
            {
                FailedVideoMetadataStorageException failedVideoMetadataStorageException =
                    new FailedVideoMetadataStorageException(
                        message: "Failed Video metadata error occured, contact support.",
                        innerException: sqlException);

                throw CreateAndLogCriticalDependencyException(failedVideoMetadataStorageException);
            }
        }

        

        private VideoMetadataValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var videoMetadataValidationException = new VideoMetadataValidationException(
                "Video Metadata Validation Exception occured, fix the errors and try again.",
                    exception);

            this.loggingBroker.LogError(videoMetadataValidationException);

            return videoMetadataValidationException;
        }
    }
}
