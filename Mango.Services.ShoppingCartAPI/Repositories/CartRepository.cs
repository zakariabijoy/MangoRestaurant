using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CartRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<bool> ClearCart(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
    {
        Cart cart  = _mapper.Map<Cart>(cartDto);

        //check if product exists in database, if not create it!
        var prodInDb = await _db.Products.FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.FirstOrDefault()!.ProductId);
        if (prodInDb == null)
        {
            await _db.Products.AddAsync(cart.CartDetails.FirstOrDefault()!.Product);
            await _db.SaveChangesAsync();
        }

       //check if header is  null
       var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);

        if (cartHeaderFromDb == null)
        {
            //create header and details
            _db.CartHeaders.Add(cart.CartHeader);
            await _db.SaveChangesAsync();
            cart.CartDetails.FirstOrDefault()!.CartHeaderId = cart.CartHeader.CartHeaderId;
            cart.CartDetails.FirstOrDefault()!.Product = null;
            _db.CartDetails.Add(cart.CartDetails.FirstOrDefault()!);
            await _db.SaveChangesAsync();
        }

        //if header is not null
        //check if details has same product 
        //if it has then update the count 
        //else create details
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
