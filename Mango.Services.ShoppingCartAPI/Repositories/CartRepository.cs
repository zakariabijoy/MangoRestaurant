using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Repositories;

public class CartRepository : ICartRepository
{
    public async Task<bool> ClearCart(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
    {
        throw new NotImplementedException();
    }

    public async Task<CartDto> GetCartByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveFromCart(int cartDetailsId)
    {
        throw new NotImplementedException();
    }
}
