// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Reelity.Core.Api.Services.VideoMetadatas;
using RESTFulSense.Controllers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading.Tasks;
using Reelity.Core.Api.Models.VideoMetadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;

namespace Reelity.Core.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoMetadatasController : RESTFulController
    {
        private readonly IVideoMetadataService videoMetadataService;

        public VideoMetadatasController(IVideoMetadataService videoMetadataService) =>
        this.videoMetadataService = videoMetadataService;

        [HttpPost]
        public async ValueTask<ActionResult<VideoMetadata>> PostVideoMetadataAsync(VideoMetadata videoMetadata)
        {
            try
            {
                VideoMetadata postedVideoMetadata = await this.videoMetadataService.AddVideoMetadataAsync(videoMetadata);

                return Created(postedVideoMetadata);
            }
            catch (VideoMetadataValidationException videoMetadataValidationException)
            {
                return BadRequest(videoMetadataValidationException.InnerException);
            }
            catch (VideoMetadataDependencyException videoMetadataDependencyException)
            {
                return InternalServerError(videoMetadataDependencyException.InnerException);
            }
            catch (VideoMetadataDependencyValidationException videoMetadataDependencyValidationException)
                when (videoMetadataDependencyValidationException.InnerException is AlreadyExitsVideoMetadataException)
            {
                return Conflict(videoMetadataDependencyValidationException.InnerException);
            }
            catch (VideoMetadataDependencyValidationException videoMetadataDependencyValidationException)
            {
                return BadRequest(videoMetadataDependencyValidationException.InnerException);
            }
            catch (VideoMetadataServiceException videoMetadataServiceException)
            {
                return InternalServerError(videoMetadataServiceException.InnerException);
            }
        }
    }
}
