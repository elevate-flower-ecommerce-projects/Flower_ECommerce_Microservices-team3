namespace Catalog_Service.Features.Categories.Queries.GetActiveCategories;

public class CategoryDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }
}