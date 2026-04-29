using Estoque.Application.DTOs.Categories;
using Estoque.Application.Services;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Repositories;
using Moq;

namespace Estoque.Application.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedCategories()
    {
        var service = CreateService();
        _categoryRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { new() { Id = 1, Nome = "Informatica" } });

        var result = await service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Informatica", result[0].Nome);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistCategory()
    {
        var service = CreateService();

        var result = await service.CreateAsync(new CategoryCreateDto { Nome = "  Casa  " });

        Assert.Equal("Casa", result.Nome);
        _categoryRepositoryMock.Verify(x => x.AddAsync(It.Is<Category>(c => c.Nome == "Casa"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenCategoryNotFound()
    {
        var service = CreateService();
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(2, new CategoryUpdateDto { Nome = "Novo" }));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingCategory()
    {
        var service = CreateService();
        var category = new Category { Id = 1, Nome = "Antigo" };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(category);

        var result = await service.UpdateAsync(1, new CategoryUpdateDto { Nome = "  Novo  " });

        Assert.Equal("Novo", result.Nome);
        _categoryRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Category>(c => c.Nome == "Novo"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrow_WhenCategoryHasLinkedProducts()
    {
        var service = CreateService();
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category { Id = 1, Nome = "Categoria" });
        _categoryRepositoryMock.Setup(x => x.HasProductsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => service.DeleteAsync(1));
        _categoryRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDelete_WhenCategoryHasNoProducts()
    {
        var service = CreateService();
        var category = new Category { Id = 1, Nome = "Categoria" };
        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        _categoryRepositoryMock.Setup(x => x.HasProductsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await service.DeleteAsync(1);

        _categoryRepositoryMock.Verify(x => x.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    private CategoryService CreateService() => new(_categoryRepositoryMock.Object);
}
