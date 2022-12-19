using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ProductRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductDto> CreateUpdateProduct(ProductDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        if(product.ProductId > 0)
        {
            _db.Products.Update(product);
        }
        else
        {
            await _db.Products.AddAsync(product);
        }

        await _db.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteProduct(int productId)
    {
        try
        {
            var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product is null)
                return false;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {

            return false;
        }
    }

    public async Task<ProductDto> GetProductById(int productId)
    {
        var product = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetProducts()
    {
        var productList = await _db.Products.ToListAsync();
        return _mapper.Map<List<ProductDto>>(productList);
    }
}
