// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Reelity.Core.Api.Models.VideoMetadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using STX.EFxceptions.Abstractions.Models.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace Reelity.Core.Api.Services.VideoMetadatas
{
    public partial class VideoMetadataService
    {
        private delegate ValueTask<VideoMetadata> ReturningVideoMetadataFunction();
        private delegate IQueryable<VideoMetadata> ReturningVideoMetadatasFunction();

        private async ValueTask<VideoMetadata> TryCatch(ReturningVideoMetadataFunction returningVideoMetadataFunction)
        {
            try
            {
                return await returningVideoMetadataFunction();
            }
            catch (NullVideoMetadataException nullVideoMetadataException)
            {
                throw CreateAndLogValidationException(nullVideoMetadataException);
            }
            catch (InvalidVideoMetadataException invalidVideoMetadataException)
            {
                throw CreateAndLogValidationException(invalidVideoMetadataException);
            }
            catch (NotFoundVideoMetadataException notFoundVideoMetadataException)
            {
                throw CreateAndLogValidationException(notFoundVideoMetadataException);
            }
            catch (SqlException sqlException)
            {
                var failedVideoMetadataStorageException =
                    new FailedVideoMetadataStorageException(
                        message: "Failed Video metadata error occured, contact support.",
                        innerException: sqlException);

                throw CreateAndLogCriticalDependencyException(failedVideoMetadataStorageException);
            }
            catch (DuplicateKeyException dublicateKeyException)
            {
                var alreadyExistsVideoMetadataException = new AlreadyExitsVideoMetadataException(
                    message: "Video metadata already exists.",
                    innerException: dublicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsVideoMetadataException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedCompanyException = new LockedVideoMetadataException(
                    message: "Video metadata is locked, try again later.",
                    innerException: dbUpdateConcurrencyException);

                throw CreateAndLogDependencyException(lockedCompanyException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedVideoMetadataStorageException =
                     new FailedVideoMetadataStorageException(
                         message: "Failed Video metadata error occured, contact support.",
                         innerException: dbUpdateException);

                throw CreateAndLogDependencyException(failedVideoMetadataStorageException);
            }
            catch (Exception serviceException)
            {
                var failedLanguageServiceException = new FailedVideoMetadataServiceException(
                    message: "Failed Video metadata service error occured, please contact support",
                    innerException: serviceException);

                throw CreateAndLogServiceException(failedLanguageServiceException);
            }
        }

        private IQueryable<VideoMetadata> TryCatch(ReturningVideoMetadatasFunction returningVideoMetadataFunction)
        {
            try
            {
                return returningVideoMetadataFunction();
            }
            catch (SqlException sqlException)
            {
                var failedVideoMetadataStorageException = new FailedVideoMetadataStorageException(
                   message: "Failed Video metadata storage exception occurred, contact support",
                   innerException: sqlException);

                throw CreateAndLogCriticalDependencyException(failedVideoMetadataStorageException);
            }
            catch (Exception serviceException)
            {
                var failedLanguageServiceException = new FailedVideoMetadataServiceException(
                    message: "Failed Video metadata service error occured, please contact support",
                    innerException: serviceException);

                throw CreateAndLogServiceException(failedLanguageServiceException);
            }
        }

        private VideoMetadataDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var videoMetadataDependencyException = new VideoMetadataDependencyException(
                message: "Video metadata dependency error occured, fix the errors and try again.",
                innerException: exception);

            this.loggingBroker.LogError(videoMetadataDependencyException);

            return videoMetadataDependencyException;
        }

        private VideoMetadataServiceException CreateAndLogServiceException(Xeption exception)
        {
            var languageServiceException = new VideoMetadataServiceException(
                message: "Video metadata service error occurred, contact support.",
                innerException: exception);

            this.loggingBroker.LogError(languageServiceException);

            return languageServiceException;
        }

        private Exception CreateAndLogDependencyValidationException(Xeption exception)
        {
            var videoMetadataDependencyValidationException = new VideoMetadataDependencyValidationException(
                message: "Video metadata Dependency validation error occured , fix the errors and try again.",
                innerException: exception);

            this.loggingBroker.LogError(videoMetadataDependencyValidationException);

            return videoMetadataDependencyValidationException;
        }

        private VideoMetadataDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var videoMetadataDependencyException = new VideoMetadataDependencyException(
                message: "Video metadata dependency error occured, fix the errors and try again.",
                innerException: exception);

            this.loggingBroker.LogCritical(videoMetadataDependencyException);

            return videoMetadataDependencyException;
        }

        private VideoMetadataValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var videoMetadataValidationException = new VideoMetadataValidationException(
                message: "Video Metadata Validation Exception occured, fix the errors and try again.",
                innerException: exception);

            this.loggingBroker.LogError(videoMetadataValidationException);

            return videoMetadataValidationException;
        }
    }
}
