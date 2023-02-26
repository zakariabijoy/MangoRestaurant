using Mango.Services.OrderAPI.Messages;

namespace Mango.Services.OrderAPI.Messagess;

public class CartDetailsDto
{
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    public int ProductId { get; set; }
    public virtual ProductDto? Product { get; set; }
    public int Count { get; set; }
}
