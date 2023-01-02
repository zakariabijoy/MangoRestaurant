using Mango.Services.ProductAPI.Models.Dtos;
using Mango.Services.ProductAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[Route("api/products")]
[ApiController]
public class ProductAPIController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    protected ResponseDto _response;

    public ProductAPIController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        _response = new ResponseDto();  
    }

    [HttpGet]
    public async Task<ResponseDto> Get()
    {
        try
        {
            IEnumerable<ProductDto> productDtos = await _productRepository.GetProducts();
            _response.Result = productDtos;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ResponseDto> Get(int id)
    {
        try
        {
            ProductDto productDto = await _productRepository.GetProductById(id);
            _response.Result = productDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpPost]
    public async Task<ResponseDto> Post([FromBody] ProductDto productDto)
    {
        try
        {
            ProductDto createdProduct = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = createdProduct;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpPut]
    public async Task<ResponseDto> Put([FromBody] ProductDto productDto)
    {
        try
        {
            ProductDto updatedProduct = await _productRepository.CreateUpdateProduct(productDto);
            _response.Result = updatedProduct;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<ResponseDto> Delete(int id)
    {
        try
        {
            bool isDeleted = await _productRepository.DeleteProduct(id);
            _response.Result = isDeleted;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { ex.ToString() };
        }

        return _response;
    }
}
