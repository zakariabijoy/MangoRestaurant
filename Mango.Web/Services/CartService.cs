using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class CartService : BaseService, ICartService
{
    private readonly IHttpClientFactory _httpClient;

    public CartService(IHttpClientFactory httpClient) : base(httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> AddToCartAsync<T>(CartDto cartDto, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/AddCart",
            AccessToken = token
        });
    }

    public async Task<T> ApplyCoupon<T>(CartDto cartDto, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/ApplyCoupon",
            AccessToken = token
        });
    }

    public async Task<T> Checkout<T>(CartHeaderDto cartHeader, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.POST,
            Data = cartHeader,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/Checkout",
            AccessToken = token
        });
    }

    public async Task<T> GetCartByUserIdAsync<T>(string userId, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.GET,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/GetCart/" + userId,
            AccessToken = token
        });
    }

    public async Task<T> RemoveCoupon<T>(string userId, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.POST,
            Data = userId,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/RemoveCoupon",
            AccessToken = token
        });
    }

    public async Task<T> RemoveFromCartAsync<T>(int cartId, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.POST,
            Data = cartId,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/RemoveCart",
            AccessToken = token
        });
    }

    public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/UpdateCart",
            AccessToken = token
        });
    }
}
