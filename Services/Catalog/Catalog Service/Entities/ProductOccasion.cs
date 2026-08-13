namespace Catalog_Service.Entities;

public class ProductOccasion
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public Guid OccasionId { get; set; }
    public Occasion Occasion { get; set; } = null!;
}
