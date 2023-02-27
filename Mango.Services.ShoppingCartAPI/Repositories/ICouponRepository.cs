using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Repositories;

public interface ICouponRepository
{
    Task<CouponDto> GetCoupon(string couponName);
}
