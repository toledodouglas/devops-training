using Estoque.Application.DTOs.Products;

namespace Estoque.Application.Services.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(ProductCreateDto request, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateAsync(int id, ProductUpdateDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto> DecreaseStockAsync(int id, int quantityToRemove, CancellationToken cancellationToken = default);
}
