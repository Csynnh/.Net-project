using System.Text.Json;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

namespace service;

// IProductService.cs
public interface IProductService
{
    Task<string> CreateProductAsync(CreateProductModel ProductRequest);
    Task<ProductModelResponse> GetProductByIdAsync(Guid id);
    Task<PagedResponse<ProductModelResponse>> ListProductByTypeNameAsync(string name, int pageNumber, int pageSize, string? size, decimal? minPrice, decimal? maxPrice);
    Task<IEnumerable<ListProductByOderStatusResponse>> ListProductByOderStatusAsync(Guid accountId, string orderStatus);
    Task<IEnumerable<ListProductByTypeResponse>> ListProductByTypeAsync();
    Task<string> UpdateProductAsync(Guid id, UpdateProductModel ProductRequest);
    Task<string> UploadImageAsync(IFormFile image);
    Task DeleteProductAsync(Guid id);
    Task DeleteProductVariantAsync(Guid id);
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
            var existingProduct = await _repository.GetProductByNameColorSizeAsync(ProductRequest.ProductName, ProductRequest.Color, ProductRequest.Size);
            var uploader = new BlodUploader();
            if (existingProduct != null)
            {
                // If the product already exists, update the adding inventory
                dynamic existingProductDynamic = existingProduct.Variants;
                System.Console.WriteLine($"{existingProduct.Id} -- Product already exists: {existingProductDynamic[0].Id}");
                var Inventory = existingProductDynamic[0].Inventory;
                var newInventory = Inventory + ProductRequest.Inventory;
                string imgThubnail = await uploader.UploadFileAsync(ProductRequest.Images.ImageThumbnail);
                List<string> updatedAdditionalImageUrls = new List<string>();
                foreach (var additionalImage in ProductRequest.Images.AdditionalImages)
                {
                    updatedAdditionalImageUrls.Add(await uploader.UploadFileAsync(additionalImage));
                }

                string updatedImages = JsonSerializer.Serialize(new ProductImagesModel()
                {
                    ImageThumbnail = imgThubnail,
                    AdditionalImages = updatedAdditionalImageUrls
                }
                );

                var newProduct = new ProductModel()
                {
                    Name = ProductRequest.ProductName,
                    Description = ProductRequest.ProductDescription,
                    Size = ProductRequest.Size.ToUpper(),
                    Color = ProductRequest.Color.ToUpper(),
                    Type = ProductRequest.Type.ToUpper(),
                    Price = ProductRequest.Price,
                    Inventory = newInventory,
                    Details = JsonSerializer.Serialize(ProductRequest.Details),
                    Images = updatedImages
                };
                await _repository.UpdateProductAsync(existingProduct.Id, existingProductDynamic[0].Id, newProduct);
                return $"Product {ProductRequest.ProductName} updated successfully with new inventory {newInventory}!";
            }
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

    public async Task<string> UpdateProductAsync(Guid id, UpdateProductModel ProductRequest)
    {
        try
        {

            var existingProduct = await _repository.GetProductVariantByIdAsync(id);
            if (existingProduct == null)
            {
                throw new InvalidOperationException($"Product with id: '{id}' does not exist");
            }
            System.Console.WriteLine($"Product found: {JsonSerializer.Serialize(ProductRequest.Images)}");
            string images = JsonSerializer.Serialize(new ProductImagesModel()
            {
                ImageThumbnail = ProductRequest.Images.ImageThumbnail,
                AdditionalImages = ProductRequest.Images.AdditionalImages
            });

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

            await _repository.UpdateProductAsync(existingProduct.Id, id, productModel);
            return $"Product {ProductRequest.ProductName} updated successfully!";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"An error occurred while updating the product: {ex.Message}");
        }
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        try
        {
            var uploader = new BlodUploader();
            return await uploader.UploadFileAsync(image);
        }
        catch (System.Exception ex)
        {
            throw new InvalidOperationException($"An error occurred while uploading the image: {ex.Message}");
        }
    }

    public async Task DeleteProductAsync(Guid id)
    {
        try
        {
            var existingProduct = await _repository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                throw new InvalidOperationException($"Product with id: '{id}' does not exist");
            }
            await _repository.DeleteProductAsync(id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"An error occurred while deleting the product: {ex.Message}");
        }
    }

    public async Task DeleteProductVariantAsync(Guid id)
    {
        try
        {
            var existingProduct = await _repository.GetProductVariantByIdAsync(id);
            if (existingProduct == null)
            {
                throw new InvalidOperationException($"Product variant with id: '{id}' does not exist");
            }

            var isLastVariant = await _repository.IsLastVariantAsync(existingProduct.Id);
            if (isLastVariant)
            {
                await _repository.DeleteProductAsync(existingProduct.Id);
                return;
            }
            await _repository.DeleteProductVariantAsync(id);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"An error occurred while deleting the product variant: {ex.Message}");
        }
    }
}
