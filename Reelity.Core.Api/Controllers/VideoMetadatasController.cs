// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Reelity.Core.Api.Models.VideoMetadatas;
using Reelity.Core.Api.Models.VideoMetadatas.Exceptions;
using Reelity.Core.Api.Services.VideoMetadatas;
using RESTFulSense.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.OData;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        [HttpGet]
        [EnableQuery]
        public ActionResult<IQueryable<VideoMetadata>> GetAllVideoMetadatas()
        {
            try
            {
                IQueryable<VideoMetadata> allVideoMetadatas = 
                    this.videoMetadataService.RetrieveAllVideoMetadatas();

                return Ok(allVideoMetadatas);
            }
            catch (VideoMetadataDependencyException videoMetadataDependencyException)
            {
                return InternalServerError(videoMetadataDependencyException.InnerException);
            }
            catch (VideoMetadataServiceException videoMetadataServiceException)
            {
                return InternalServerError(videoMetadataServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<VideoMetadata>> PutVideoMetadataAsync(VideoMetadata videoMetadata)
        {
            try
            {
                VideoMetadata modifiedVideoMetadata = await this.videoMetadataService.ModifyVideoMetadataAsync(videoMetadata);

                return Ok(modifiedVideoMetadata);
            }
            catch (VideoMetadataValidationException videoMetadataValidationException)
                  when (videoMetadataValidationException.InnerException is NotFoundVideoMetadataException)
            {
                return NotFound(videoMetadataValidationException.InnerException);
            }
            catch (VideoMetadataValidationException videoMetadataValidationException)
            {
                return BadRequest(videoMetadataValidationException.InnerException);
            }
            catch (VideoMetadataDependencyException videoMetadataDependencyException)
            {
                return InternalServerError(videoMetadataDependencyException.InnerException);
            }
            catch (VideoMetadataServiceException videoMetadataServiceException)
            {
                return InternalServerError(videoMetadataServiceException.InnerException);
            }
        }

        [HttpDelete]
        public async ValueTask<ActionResult<VideoMetadata>> DeleteVideoMetadataByIdAsync(Guid id)
        {
            try
            {
                VideoMetadata deletedVideoMetadata = await this.videoMetadataService.DeleteVideoMetadataAsync(id);

                return Ok(deletedVideoMetadata);
            }
            catch (VideoMetadataValidationException videoMetadataValidationException)
                  when (videoMetadataValidationException.InnerException is NotFoundVideoMetadataException)
            {
                return NotFound(videoMetadataValidationException.InnerException);
            }
            catch (VideoMetadataValidationException videoMetadataValidationException)
            {
                return BadRequest(videoMetadataValidationException.InnerException);
            }
            catch (VideoMetadataDependencyValidationException videoMetadataDependencyValidationException)
                when (videoMetadataDependencyValidationException.InnerException is LockedVideoMetadataException)
            {
                return Locked(videoMetadataDependencyValidationException.InnerException);
            }
            catch (VideoMetadataDependencyValidationException videoMetadataDependencyValidationException)
            {
                return BadRequest(videoMetadataDependencyValidationException.InnerException);
            }
            catch (VideoMetadataDependencyException videoMetadataDependencyException)
            {
                return InternalServerError(videoMetadataDependencyException.InnerException);
            }
            catch (VideoMetadataServiceException videoMetadataServiceException)
            {
                return InternalServerError(videoMetadataServiceException.InnerException);
            }
        }
    }
}
