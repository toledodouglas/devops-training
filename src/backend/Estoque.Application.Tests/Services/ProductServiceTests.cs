using Estoque.Application.DTOs.Products;
using Estoque.Application.Services;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Repositories;
using Moq;

namespace Estoque.Application.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedProducts()
    {
        var service = CreateService();
        _productRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { new() { Id = 1, Nome = "Mouse", Quantidade = 10, CategoryId = 2 } });

        var result = await service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Mouse", result[0].Nome);
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnProductsByCategory()
    {
        var service = CreateService();
        _productRepositoryMock.Setup(x => x.GetByCategoryAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { new() { Id = 1, Nome = "Mouse", Quantidade = 10, CategoryId = 2 } });

        var result = await service.GetByCategoryAsync(2);

        Assert.Single(result);
        Assert.Equal(2, result[0].CategoryId);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCategoryDoesNotExist()
    {
        var service = CreateService();
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.CreateAsync(new ProductCreateDto { Nome = "Teclado", Quantidade = 5, CategoryId = 5 }));
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistProduct_WhenCategoryExists()
    {
        var service = CreateService();
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category { Id = 1, Nome = "Informatica" });

        var result = await service.CreateAsync(new ProductCreateDto { Nome = "  Teclado  ", Quantidade = 5, CategoryId = 1 });

        Assert.Equal("Teclado", result.Nome);
        _productRepositoryMock.Verify(x => x.AddAsync(It.Is<Product>(p => p.Nome == "Teclado"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenProductNotFound()
    {
        var service = CreateService();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(8, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateAsync(8, new ProductUpdateDto { Nome = "X", Quantidade = 1, CategoryId = 1 }));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenDataIsValid()
    {
        var service = CreateService();
        var product = new Product { Id = 1, Nome = "Antigo", Quantidade = 1, CategoryId = 1 };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category { Id = 2, Nome = "Casa" });

        var result = await service.UpdateAsync(1, new ProductUpdateDto { Nome = "Novo", Quantidade = 20, CategoryId = 2 });

        Assert.Equal("Novo", result.Nome);
        Assert.Equal(20, result.Quantidade);
        _productRepositoryMock.Verify(x => x.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenProductNotFound()
    {
        var service = CreateService();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.DeleteAsync(2));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenProductExists()
    {
        var service = CreateService();
        var product = new Product { Id = 2, Nome = "Monitor", Quantidade = 3, CategoryId = 1 };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        await service.DeleteAsync(2);

        _productRepositoryMock.Verify(x => x.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DecreaseStockAsync_ShouldThrow_WhenQuantityIsInvalid()
    {
        var service = CreateService();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = 1, Nome = "Mouse", Quantidade = 10, CategoryId = 1 });

        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => service.DecreaseStockAsync(1, 0));
    }

    [Fact]
    public async Task DecreaseStockAsync_ShouldThrow_WhenQuantityWouldBecomeNegative()
    {
        var service = CreateService();
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product { Id = 1, Nome = "Mouse", Quantidade = 2, CategoryId = 1 });

        await Assert.ThrowsAsync<NegativeStockException>(() => service.DecreaseStockAsync(1, 3));
    }

    [Fact]
    public async Task DecreaseStockAsync_ShouldUpdateStock_WhenQuantityIsValid()
    {
        var service = CreateService();
        var product = new Product { Id = 1, Nome = "Mouse", Quantidade = 10, CategoryId = 1 };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var result = await service.DecreaseStockAsync(1, 4);

        Assert.Equal(6, result.Quantidade);
        _productRepositoryMock.Verify(x => x.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    private ProductService CreateService() => new(_productRepositoryMock.Object, _categoryRepositoryMock.Object);
}
