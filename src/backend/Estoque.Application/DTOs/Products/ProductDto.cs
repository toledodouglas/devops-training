namespace Estoque.Application.DTOs.Products;

public sealed class ProductDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public int CategoryId { get; init; }
    public string? CategoriaNome { get; init; }
}
