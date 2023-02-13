﻿using Mango.Web.Models.Dtos;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;

    public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
    {
        _productService = productService;
        _cartService = cartService;
        _couponService = couponService;
    }

    public async Task<IActionResult> CartIndex()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }

    [HttpPost]
    [ActionName("ApplyCoupon")]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, accessToken!);

        if (response is not null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    [HttpPost]
    [ActionName("RemoveCoupon")]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.CartHeader.UserId, accessToken!);

        if (response is not null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailsId, accessToken!);

        if (response is not null && response.IsSuccess)
        {
            return RedirectToAction(nameof(CartIndex));
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
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

        if (cartDto.CartHeader is not null)
        {
            if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
            {
                var coupon = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode,accessToken!);
                if (coupon is not null && coupon.IsSuccess)
                {
                    var couponObj = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(coupon.Result)!)!;
                    cartDto.CartHeader.DiscountTotal = couponObj.DiscountAmount;
                }
            }

            foreach (var details in cartDto.CartDetails)
            {
                cartDto.CartHeader.OrderTotal += (details.Product.Price * details.Count);
            }

            cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
        }
        return cartDto;
    }
}
