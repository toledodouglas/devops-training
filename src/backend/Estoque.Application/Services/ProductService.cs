using Estoque.Application.DTOs.Products;
using Estoque.Application.Services.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Repositories;

namespace Estoque.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId, cancellationToken);
        return products.Select(Map).ToList();
    }

    public async Task<ProductDto> CreateAsync(ProductCreateDto request, CancellationToken cancellationToken = default)
    {
        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        var product = new Product
        {
            Nome = request.Nome.Trim(),
            Quantidade = request.Quantidade,
            CategoryId = request.CategoryId
        };

        await _productRepository.AddAsync(product, cancellationToken);
        return Map(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductUpdateDto request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Produto não encontrado.");

        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        product.Nome = request.Nome.Trim();
        product.Quantidade = request.Quantidade;
        product.CategoryId = request.CategoryId;

        await _productRepository.UpdateAsync(product, cancellationToken);
        return Map(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Produto não encontrado.");

        await _productRepository.DeleteAsync(product, cancellationToken);
    }

    public async Task<ProductDto> DecreaseStockAsync(int id, int quantityToRemove, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Produto não encontrado.");

        if (quantityToRemove <= 0)
        {
            throw new BusinessRuleViolationException("Quantidade removida deve ser maior que zero.");
        }

        if (quantityToRemove > product.Quantidade)
        {
            throw new NegativeStockException(product.Nome, quantityToRemove, product.Quantidade);
        }

        product.Quantidade -= quantityToRemove;
        await _productRepository.UpdateAsync(product, cancellationToken);
        return Map(product);
    }

    private async Task EnsureCategoryExistsAsync(int categoryId, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException("Categoria não encontrada.");
        }
    }

    private static ProductDto Map(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Nome = product.Nome,
            Quantidade = product.Quantidade,
            CategoryId = product.CategoryId,
            CategoriaNome = product.Category?.Nome
        };
    }
}
