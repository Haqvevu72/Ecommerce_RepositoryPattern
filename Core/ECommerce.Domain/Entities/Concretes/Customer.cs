using ECommerce.Domain.Entities.Common;

namespace ECommerce.Domain.Entities.Concretes;

public class Customer:BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }

    // Navigation Property
    public  virtual ICollection<Order> Orders { get; set; }
}
