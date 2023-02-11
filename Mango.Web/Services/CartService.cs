﻿using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.IServices;
using System.Net.Http;

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

    public async Task<T> GetCartByUserIdAsync<T>(int userId, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.GET,
            Url = SD.ShoppingCartAPIBase + "/api/Cart/GetCart/" + userId,
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