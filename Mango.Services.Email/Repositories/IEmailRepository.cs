using Mango.Services.OrderAPI.Models;

namespace Mango.Services.Email.Repositories;

public interface IEmailRepository
{
    Task<bool> AddOrder(OrderHeader orderHeader);
    Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid);
}
