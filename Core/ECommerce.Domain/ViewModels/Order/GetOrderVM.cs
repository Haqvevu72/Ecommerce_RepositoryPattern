namespace ECommerce.Domain.ViewModels.Order;

public class GetOrderVM
{
    public string? OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public string? OrderNote { get; set; }
    public decimal Total { get; set; }

    // Foreign Key
    public int CustomerId { get; set; }
}