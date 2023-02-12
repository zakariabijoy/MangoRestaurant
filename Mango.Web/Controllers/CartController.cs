using Mango.Web.Models.Dtos;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public CartController(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }

    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId!,accessToken!);

        CartDto cartDto = new();
        if(response is not null && response.IsSuccess)
        {
            cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result)!)!;
        }

        if(cartDto.CartHeader is not null)
        {
            foreach (var details in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (details.Product.Price * details.Count);
            }
        }
        return cartDto;
    }
}
