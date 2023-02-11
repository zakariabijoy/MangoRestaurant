using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> products = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if(response is not null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto product = new();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId,"");
            if (response is not null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }

            return View(product);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            CartDto cartDto = new()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetails = new()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,
            };

            var resp = await _productService.GetProductByIdAsync<ResponseDto>(productDto.ProductId, "");

            if(resp is not null && resp.IsSuccess)
            {
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result)!)!;  
            }

            List<CartDetailsDto> cartDetailsDtos = new() { cartDetails};
            cartDto.CartDetails= cartDetailsDtos;

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addToCartResp = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken!);
            if (addToCartResp is not null && addToCartResp.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productDto);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult Login()
        {            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}