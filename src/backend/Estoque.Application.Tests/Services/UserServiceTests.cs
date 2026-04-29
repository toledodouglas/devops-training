using Estoque.Application.Abstractions.Security;
using Estoque.Application.DTOs.Users;
using Estoque.Application.Services;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Repositories;
using Moq;

namespace Estoque.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenDataIsValid()
    {
        var service = CreateService();
        var dto = new UserCreateDto
        {
            NomeCompleto = "Maria Silva",
            Idade = 24,
            Email = "MARIA@MAIL.COM",
            Senha = "123456"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasherMock.Setup(x => x.Hash(dto.Senha)).Returns("hashed");

        var result = await service.CreateAsync(dto);

        Assert.Equal("maria@mail.com", result.Email);
        Assert.Equal("Maria Silva", result.NomeCompleto);
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => u.SenhaHash == "hashed"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        var service = CreateService();
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Email = "used@mail.com" });

        var act = () => service.CreateAsync(new UserCreateDto
        {
            NomeCompleto = "Joao",
            Idade = 22,
            Email = "used@mail.com",
            Senha = "abc"
        });

        await Assert.ThrowsAsync<BusinessRuleViolationException>(act);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenAgeIsLowerThan18()
    {
        var service = CreateService();
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var act = () => service.CreateAsync(new UserCreateDto
        {
            NomeCompleto = "Ana",
            Idade = 17,
            Email = "ana@mail.com",
            Senha = "abc"
        });

        await Assert.ThrowsAsync<BusinessRuleViolationException>(act);
        _passwordHasherMock.Verify(x => x.Hash(It.IsAny<string>()), Times.Never);
    }

    private UserService CreateService() => new(_userRepositoryMock.Object, _passwordHasherMock.Object);
}
