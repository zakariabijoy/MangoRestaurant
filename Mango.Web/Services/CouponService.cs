using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services;

public class CouponService : BaseService, ICouponService
{
    private readonly IHttpClientFactory _httpClient;

    public CouponService(IHttpClientFactory httpClient) : base(httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetCoupon<T>(string couponCode, string token)
    {
        return await SendAsync<T>(new APIRequest
        {
            ApiType = SD.APIType.GET,
            Url = SD.CouponAPIBase + "/api/coupon/" + couponCode,
            AccessToken = token
        });
    }
}
