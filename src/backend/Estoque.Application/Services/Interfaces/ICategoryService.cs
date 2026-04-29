using Estoque.Application.DTOs.Categories;

namespace Estoque.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateAsync(CategoryCreateDto request, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateAsync(int id, CategoryUpdateDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
