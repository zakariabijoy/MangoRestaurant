using Mango.Web.Models;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.IServices;
using System.Text;
using System.Text.Json;

namespace Mango.Web.Services
{
    public class BaseServie : IBaseService
    {
        public ResponseDto ResponseDto { get; set; }
        private readonly IHttpClientFactory _httpClient;
     
        public BaseServie(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = _httpClient.CreateClient(("MangoAPI"));
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
                client.DefaultRequestHeaders.Clear();
                if(apiRequest.Data is not null)
                {
                    message.Content = new StringContent(JsonSerializer.Serialize(apiRequest.Data),Encoding.UTF8, "application/json");
                }

                HttpResponseMessage apiResponse = null;
                switch (apiRequest.ApiType)
                {  
                    
                    case SD.APIType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.APIType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.APIType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiResponseDto =  JsonSerializer.Deserialize<T>(apiContent);
                return apiResponseDto;
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    DisplayMessage = "Error",
                    ErrorMessages = new List<string> { ex.Message.ToString() },
                    IsSuccess = false
                };
                var res = JsonSerializer.Serialize(dto);
                var apiResponseDto = JsonSerializer.Deserialize<T>(res);
                return apiResponseDto;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
