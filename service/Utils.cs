
using System.Security.Claims;
using Azure.Storage.Blobs;
using infrastructure.DataModels;
using infrastructure.Repositories;
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

public class Authorization
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UserRepository _userRepository;
  public Authorization(IHttpContextAccessor httpContextAccessor, UserRepository userRepository)
  {
    _httpContextAccessor = httpContextAccessor;
    _userRepository = userRepository;
  }
  public async Task<bool> ValidateUser(Guid accountId)
  {
    var user = _httpContextAccessor.HttpContext?.User;
    string UsernameClaim = user?.FindFirst(ClaimTypes.Name)?.Value!;
    string RoleClaim = user?.FindFirst(ClaimTypes.Role)?.Value!;
    User? AccountRequest = await _userRepository.GetUserByAccountIdAsync(accountId);
    if (AccountRequest != null && AccountRequest.Username != UsernameClaim && RoleClaim != "Admin")
    {
      throw new UnauthorizedAccessException("You do not have permission to list this user info");
    }
    return true;
  }
}