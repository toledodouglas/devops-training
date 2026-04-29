using Estoque.Application.DTOs.Categories;
using Estoque.Application.Services.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Repositories;

namespace Estoque.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(Map).ToList();
    }

    public async Task<CategoryDto> CreateAsync(CategoryCreateDto request, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Nome = request.Nome.Trim()
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        return Map(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, CategoryUpdateDto request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        category.Nome = request.Nome.Trim();

        await _categoryRepository.UpdateAsync(category, cancellationToken);
        return Map(category);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Categoria não encontrada.");

        var hasProducts = await _categoryRepository.HasProductsAsync(id, cancellationToken);
        if (hasProducts)
        {
            throw new BusinessRuleViolationException("Não é permitido excluir categoria com produtos vinculados.");
        }

        await _categoryRepository.DeleteAsync(category, cancellationToken);
    }

    private static CategoryDto Map(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Nome = category.Nome
        };
    }
}
