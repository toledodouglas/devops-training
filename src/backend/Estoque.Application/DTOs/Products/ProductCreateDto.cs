namespace Estoque.Application.DTOs.Products;

public sealed class ProductCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public int CategoryId { get; set; }
}
