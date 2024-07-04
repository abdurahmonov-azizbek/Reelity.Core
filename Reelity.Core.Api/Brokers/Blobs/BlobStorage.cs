// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reelity.Core.Api.Models.Blobs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Reelity.Core.Api.Brokers.Blobs
{
    public class BlobStorage 
    {
        private readonly string storageConnectionString;
        private readonly string storageContainerName;
        private readonly ILogger<BlobStorage> logger;

        public BlobStorage(IConfiguration configuration, ILogger<BlobStorage> logger)
        {
            this.storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            this.storageContainerName = configuration.GetValue<string>("BlobContainerName");
            this.logger = logger;
        }

        public async Task<BlobResponse> DeleteAsync(string blobFilename)
        {
            BlobContainerClient client = new BlobContainerClient(storageConnectionString, storageContainerName);

            BlobClient file = client.GetBlobClient(blobFilename);

            try
            {
                await file.DeleteAsync();
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                logger.LogError($"File {blobFilename} was not found.");
                return new BlobResponse { Error = true, Status = $"File with name {blobFilename} not found." };
            }

            return new BlobResponse { Error = false, Status = $"File: {blobFilename} has been successfully deleted." };
        }

        public async Task<Blob> DownloadAsync(string blobFilename)
        {
            BlobContainerClient client = new BlobContainerClient(storageConnectionString, storageContainerName);

            try
            {
                BlobClient file = client.GetBlobClient(blobFilename);

                if (await file.ExistsAsync())
                {
                    var data = await file.OpenReadAsync();
                    Stream blobContent = data;
                    var content = await file.DownloadContentAsync();
                    string name = blobFilename;
                    string contentType = content.Value.Details.ContentType;

                    return new Blob { Content = blobContent, Name = name, ContentType = contentType };
                }
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                logger.LogError($"File {blobFilename} was not found.");
            }

            return null;
        }

        public async Task<List<Blob>> ListAsync()
        {
            BlobContainerClient container = new BlobContainerClient(storageConnectionString, storageContainerName);

            List<Blob> files = new List<Blob>();

            await foreach (BlobItem file in container.GetBlobsAsync())
            {
                string uri = container.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new Blob
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType
                });
            }

            return files;
        }

        public async Task<BlobResponse> UploadAsync(IFormFile blob)
        {
            BlobResponse response = new();
            BlobContainerClient container = new BlobContainerClient(storageConnectionString, storageContainerName);
            await container.CreateIfNotExistsAsync();

            try
            {
                BlobClient client = container.GetBlobClient(blob.FileName);

                await using (Stream? data = blob.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }

                response.Status = $"File {blob.FileName} Uploaded Successfully";
                response.Error = false;
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;

            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
            {
                logger.LogError($"File with name {blob.FileName} already exists in container. Set another name to store the file in the container: '{storageContainerName}.'");
                response.Status = $"File with name {blob.FileName} already exists. Please use another name to store your file.";
                response.Error = true;
                return response;
            }
            catch (RequestFailedException ex)
            {
                logger.LogError($"Unhandled Exception. ID: {ex.StackTrace} - Message: {ex.Message}");
                response.Status = $"Unexpected error: {ex.StackTrace}. Check log with StackTrace ID.";
                response.Error = true;
                return response;
            }

            return response;
        }
    }
}
