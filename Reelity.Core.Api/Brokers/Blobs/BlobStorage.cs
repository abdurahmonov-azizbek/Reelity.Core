// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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

        public BlobStorage(IConfiguration configuration)
        {
            this.storageConnectionString = configuration.GetValue<string>("BlobConnectionString");
            this.storageContainerName = configuration.GetValue<string>("BlobContainerName");
        }

        public async Task<BlobResponse> DeleteAsync(string blobFilename)
        {
            BlobContainerClient client = new BlobContainerClient(storageConnectionString, storageContainerName);

            BlobClient file = client.GetBlobClient(blobFilename);
            await file.DeleteAsync();

            return new BlobResponse { Error = false, Status = $"File: {blobFilename} has been successfully deleted." };
        }

        public async Task<Blob> DownloadAsync(string blobFilename)
        {
            BlobContainerClient client = new BlobContainerClient(storageConnectionString, storageContainerName);
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

            return null;
        }

        public async Task<List<Blob>> GetAllBlobsAsync()
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
            BlobClient client = container.GetBlobClient(blob.FileName);

            await using (Stream data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.FileName} Uploaded Successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

            return response;
        }
    }
}
