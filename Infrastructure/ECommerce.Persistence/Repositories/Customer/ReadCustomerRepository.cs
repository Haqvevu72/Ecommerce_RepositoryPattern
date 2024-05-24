using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities.Concretes;
using ECommerce.Persistence.DbContexts;
using ECommerce.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Repositories;

public class ReadCustomerRepository : ReadGenericRepository<Customer>, IReadCustomerRepository
{
    public ReadCustomerRepository(ECommerceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersOfCustomer(int customerId)
    {
        var orders = _table.Include(c => c.Orders)
            .FirstOrDefault(c => c.Id == customerId).Orders;
        return orders;
    }
}
