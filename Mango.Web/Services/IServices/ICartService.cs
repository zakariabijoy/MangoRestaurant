﻿using Mango.Web.Models.Dtos;

namespace Mango.Web.Services.IServices;

public interface ICartService
{
    Task<T> GetCartByUserIdAsync<T>(int userId, string token );
    Task<T> AddToCartAsync<T>(CartDto cartDto, string token);
    Task<T> UpdateCartAsync<T>(CartDto cartDto, string token);
    Task<T> RemoveFromCartAsync<T>(int cartId, string token);
}