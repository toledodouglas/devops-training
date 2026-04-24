namespace Estoque.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
