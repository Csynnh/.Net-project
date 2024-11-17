
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

public class BlodUploader
{
  private readonly string _connectionString = Environment.GetEnvironmentVariable("BLOD_CONNECTION_STRING")!;
  private readonly string _containerName = Environment.GetEnvironmentVariable("BLOD_CONTAINER_NAME") ?? "images";
  public BlodUploader()
  {
  }
  public async Task<string> UploadFileAsync(IFormFile file)
  {
    BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

    // Get a reference to the container
    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

    // Ensure the container exists
    await containerClient.CreateIfNotExistsAsync();

    // Create a unique name for the blob (file) to avoid naming conflicts
    string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

    // Get a reference to the blob (image)
    BlobClient blobClient = containerClient.GetBlobClient(blobName);

    // Upload the file to Azure Blob Storage
    using (var stream = file.OpenReadStream())
    {
      await blobClient.UploadAsync(stream, overwrite: true);
    }

    // Generate the public URL for the uploaded image
    Uri blobUrl = blobClient.Uri;
    return blobUrl.ToString();
  }
}
