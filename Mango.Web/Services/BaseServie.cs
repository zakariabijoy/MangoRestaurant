using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class BaseServie : IBaseService
    {
        public ResponseDto ResponseDto { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<T> SendAsync<T>(APIRequest aPIRequest)
        {
            throw new NotImplementedException();
        }
    }
}
