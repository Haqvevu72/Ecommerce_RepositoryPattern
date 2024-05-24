using ECommerce.Domain.Entities.Concretes;

namespace ECommerce.Application.Repositories;

public interface IReadCustomerRepository : IReadGenericRepository<Customer>
{
    Task<IEnumerable<Order>> GetOrdersOfCustomer(int customerId);
}
