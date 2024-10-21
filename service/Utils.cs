using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

public class S3Uploader
{
  private readonly string bucketName = "noir-shop";
  private readonly string region = "ap-southeast-2";
  private readonly IAmazonS3 s3Client;

  public S3Uploader(IAmazonS3 _s3Client)
  {
    s3Client = _s3Client;
  }

  public static string GetContentType(string fileName)
  {
    var extension = Path.GetExtension(fileName).ToLowerInvariant();
    return extension switch
    {
      ".jpg" => "image/jpeg",
      ".jpeg" => "image/jpeg",
      ".png" => "image/png",
      ".gif" => "image/gif",
      ".bmp" => "image/bmp",
      ".pdf" => "application/pdf",
      ".doc" => "application/msword",
      ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
      ".xls" => "application/vnd.ms-excel",
      ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      _ => "application/octet-stream",
    };
  }

  public async Task<string> UploadImageAsync(IFormFile file)
  {
    try
    {
      var prefix = "products";
      var bucketExists = await s3Client.DoesS3BucketExistAsync(bucketName);
      if (!bucketExists)
        throw new Exception($"Bucket {bucketName} does not exist.");
      var request = new PutObjectRequest()
      {
        BucketName = bucketName,
        Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
        InputStream = file.OpenReadStream()
      };
      request.Metadata.Add("Content-Type", file.ContentType);
      await s3Client.PutObjectAsync(request);

      // Get the URL of the uploaded file
      var urlRequest = new GetPreSignedUrlRequest()
      {
        BucketName = bucketName,
        Key = $"{prefix?.TrimEnd('/')}/{file.FileName}",
        Expires = DateTime.UtcNow.AddMonths(2)
      };
      string publicUrl = s3Client.GetPreSignedURL(urlRequest);
      return publicUrl;
    }
    catch (Exception ex)
    {
      throw new Exception($"An error occurred: {ex.Message}");
    }
  }
}
