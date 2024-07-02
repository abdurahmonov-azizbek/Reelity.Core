// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reelity.Core.Api.Brokers.Blobs;
using Reelity.Core.Api.Models.Blobs;
using RESTFulSense.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reelity.Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : RESTFulController
    {
        private readonly BlobStorage blobStorage;

        public StorageController(BlobStorage blobStorage)
        {
            this.blobStorage = blobStorage;
        }

        [HttpGet(nameof(Get))]
        public async Task<IActionResult> Get()
        {
            List<Blob> files = await blobStorage.ListAsync();

            return StatusCode(StatusCodes.Status200OK, files);
        }

        [HttpPost(nameof(Upload))]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            BlobResponse response = await blobStorage.UploadAsync(file);

            if (response.Error == true)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, response);
            }
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> Download(string filename)
        {
            Blob file = await blobStorage.DownloadAsync(filename);

            if (file == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"File {filename} could not be downloaded.");
            }
            else
            {
                return File(file.Content, file.ContentType, file.Name);
            }
        }

        [HttpDelete("filename")]
        public async Task<IActionResult> Delete(string filename)
        {
            BlobResponse response = await blobStorage.DeleteAsync(filename);

            if (response.Error == true)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Status);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, response.Status);
            }
        }
    }
}
