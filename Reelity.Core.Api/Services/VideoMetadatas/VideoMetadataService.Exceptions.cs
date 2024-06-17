// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using FluentAssertions;
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
            catch (SqlException sqlException)
            {
                FailedVideoMetadataStorageException failedVideoMetadataStorageException =
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
                var lockedJobException = new LockedVideoMetadataException(
                    message: "VideoMetadata is locked, please try again.", 
                    innerException: dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedJobException);
            }
            catch (NotFoundVideoMetadataException notFoundVideoMetadataException)
            {
                var failedNotFoundVideoMetadataException = new NotFoundVideoMetadataException(
                  message: $"Couldn't find VideoMetadata with id.");

                throw CreateAndLogValidationException(failedNotFoundVideoMetadataException);
            }
            catch (Exception serviceException)
            {
                var failedLanguageServiceException = new FailedVideoMetadataServiceException(
                    message: "Failed Video metadata service error occured, please contact support",
                    innerException: serviceException);

                throw CreateAndLogServiceException(failedLanguageServiceException);
            }
        }

        private IQueryable<VideoMetadata> TryCatch(ReturningVideoMetadatasFunction returningVideoMetadatasFunction)
        {
            try
            {
                return returningVideoMetadatasFunction();
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

        private VideoMetadataServiceException CreateAndLogServiceException(Xeption exception)
        {
            var languageServiceException = new VideoMetadataServiceException(
                "Video metadata service error occurred, contact support.",
                exception);

            this.loggingBroker.LogError(languageServiceException);

            return languageServiceException;
        }

        private Exception CreateAndLogDependencyValidationException(Xeption exception)
        {
            var videoMetadataDependencyValidationException = new VideoMetadataDependencyValidationException(
                message: "Video metadata dependency error occured, fix the errors and try again.",
                innerException: exception);

            this.loggingBroker.LogError(videoMetadataDependencyValidationException);

            return videoMetadataDependencyValidationException;
        }

        private VideoMetadataDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var videoMetadataDependencyException = new VideoMetadataDependencyException(
                "Video metadata dependency error occured, fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogCritical(videoMetadataDependencyException);

            return videoMetadataDependencyException;
        }

        private VideoMetadataValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var videoMetadataValidationException = new VideoMetadataValidationException(
                "Video Metadata Validation Exception occured, fix the errors and try again.",
                 innerException: exception);

            this.loggingBroker.LogError(videoMetadataValidationException);

            return videoMetadataValidationException;
        }
    }
}
