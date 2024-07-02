// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

namespace Reelity.Core.Api.Models.Blobs
{
    public class BlobResponse
    {
        public string Status { get; set; }
        public bool Error { get; set; }
        public Blob Blob { get; set; }
    }
}
