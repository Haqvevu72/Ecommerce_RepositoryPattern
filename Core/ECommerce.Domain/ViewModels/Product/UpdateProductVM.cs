namespace ECommerce.Domain.ViewModels;

public class UpdateProductVM
{
    public int Id { get; set; }
    public string? Name { get; set; }

    // Foreign Key
    public int CategoryId { get; set; }
}