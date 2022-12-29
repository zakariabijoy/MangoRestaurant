using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class ProductService : BaseServie, IProductService
{
    private readonly IHttpClientFactory _httpClient;

    public ProductService(IHttpClientFactory httpClient) : base(httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<T> CreateProductAsync<T>(ProductDto productDto)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.POST,
            Data = productDto,
            Url = SD.ProductAPIBase + "/api/products",
            AccessToken = ""
        });
    }

    public async Task<T> DeleteProductAsync<T>(int id)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.DELETE,
            Url = SD.ProductAPIBase + "/api/products/" + id,
            AccessToken = ""
        });
    }

    public async Task<T> GetAllProductsAsync<T>()
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.GET,
            Url = SD.ProductAPIBase + "/api/products",
            AccessToken = ""
        });
    }

    public async Task<T> GetProductByIdAsync<T>(int id)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.GET,
            Url = SD.ProductAPIBase + "/api/products/" + id,
            AccessToken = ""
        });
    }

    public async Task<T> UpdateProductAsync<T>(ProductDto productDto)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.PUT,
            Data = productDto,
            Url = SD.ProductAPIBase + "/api/products",
            AccessToken = ""
        });
    }
}
