using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Repositories;

public class CouponRepository : ICouponRepository
{
    public Task<CouponDto> GetCoupon(string couponName)
    {
        throw new NotImplementedException();
    }
}
