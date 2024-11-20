using System.Text.Json;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

// IProductService.cs
public interface IProductService
{
    Task<string> CreateProductAsync(CreateProductModel ProductRequest);
    Task<ProductModelResponse> GetProductByIdAsync(Guid id);
    Task<PagedResponse<ProductModelResponse>> ListProductByTypeNameAsync(string name, int pageNumber, int pageSize, string? size, decimal? minPrice, decimal? maxPrice);
    Task<IEnumerable<ListProductByOderStatusResponse>> ListProductByOderStatusAsync(Guid accountId, string orderStatus);
    Task<IEnumerable<ListProductByTypeResponse>> ListProductByTypeAsync();
}


public class ProductService : IProductService
{
    private readonly ProductRepository _repository;

    public ProductService(ProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductModelResponse> GetProductByIdAsync(Guid id)
    {
        var response = await _repository.GetProductByIdAsync(id);
        return response;
    }

    public async Task<Guid> GetIdByName(string name)
    {
        var response = await _repository.GetIdByName(name);
        return response;
    }

    public async Task<PagedResponse<ProductModelResponse>> ListProductByTypeNameAsync(string name, int pageNumber, int pageSize, string? size, decimal? minPrice, decimal? maxPrice)
    {
        return await _repository.ListProductByTypeNameAsync(name, pageNumber, pageSize, size, minPrice, maxPrice);
    }



    public async Task<IEnumerable<ListProductByTypeResponse>> ListProductByTypeAsync()
    {
        var response = await _repository.ListProductByTypeAsync();
        return response;
    }

    public async Task<IEnumerable<ListProductByOderStatusResponse>> ListProductByOderStatusAsync(Guid accountId, string orderStatus)
    {
        var response = await _repository.ListProductByOderStatusAsync(accountId, orderStatus);
        return response;
    }
    public async Task<string> CreateProductAsync(CreateProductModel ProductRequest)
    {
        try
        {
            // Check if the product already exists based on your criteria
            var existingProduct = await _repository.IsProductExistAsync(ProductRequest.ProductName, ProductRequest.Color, ProductRequest.Size);
            if (existingProduct)
            {
                throw new InvalidOperationException($"Product with name: '{ProductRequest.ProductName}' - color: '{ProductRequest.Color}' - size '{ProductRequest.Size}' already exists");
            }
            var uploader = new BlodUploader();
            string image_url = await uploader.UploadFileAsync(ProductRequest.Images.ImageThumbnail);
            List<string> additionalImageUrls = new List<string>();
            foreach (var additionalImage in ProductRequest.Images.AdditionalImages)
            {
                additionalImageUrls.Add(await uploader.UploadFileAsync(additionalImage));
            }

            string images = JsonSerializer.Serialize(new ProductImagesModel()
            {
                ImageThumbnail = image_url,
                AdditionalImages = additionalImageUrls
            }
            );

            ProductModel productModel = new ProductModel()
            {
                Name = ProductRequest.ProductName,
                Description = ProductRequest.ProductDescription,
                Size = ProductRequest.Size.ToUpper(),
                Color = ProductRequest.Color.ToUpper(),
                Type = ProductRequest.Type.ToUpper(),
                Price = ProductRequest.Price,
                Inventory = ProductRequest.Inventory,
                Details = JsonSerializer.Serialize(ProductRequest.Details),
                Images = images
            };



            await _repository.AddProductAsync(productModel);
            return $"Product created successfully with name: {ProductRequest.ProductName}";
        }
        catch (Exception ex)
        {

            throw new InvalidOperationException($"An error occurred while creating the product: {ex.Message}");
        }
    }

}
