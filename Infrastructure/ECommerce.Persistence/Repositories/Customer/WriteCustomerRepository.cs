using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities.Concretes;
using ECommerce.Persistence.DbContexts;
using ECommerce.Persistence.Repositories.Common;

namespace ECommerce.Persistence.Repositories;

public class WriteCustomerRepository : WriteGenericRepository<Customer>, IWriteCustomerRepository
{
    public WriteCustomerRepository(ECommerceDbContext context) : base(context)
    {
    }
}

