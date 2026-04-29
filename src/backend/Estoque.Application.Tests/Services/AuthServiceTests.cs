using Estoque.Application.Abstractions.Security;
using Estoque.Application.DTOs.Auth;
using Estoque.Application.Services;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Repositories;
using Moq;

namespace Estoque.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var service = CreateService();
        var user = new User { Email = "user@mail.com", SenhaHash = "hash", NomeCompleto = "User" };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync("user@mail.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(x => x.Verify("123", "hash"))
            .Returns(true);
        _tokenServiceMock
            .Setup(x => x.GenerateToken(user))
            .Returns("jwt-token");

        var result = await service.LoginAsync(new LoginRequestDto { Email = "User@Mail.com", Senha = "123" });

        Assert.Equal("jwt-token", result.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenUserIsNotFound()
    {
        var service = CreateService();
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var act = () => service.LoginAsync(new LoginRequestDto { Email = "none@mail.com", Senha = "123" });

        await Assert.ThrowsAsync<BusinessRuleViolationException>(act);
        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsInvalid()
    {
        var service = CreateService();
        var user = new User { Email = "user@mail.com", SenhaHash = "hash" };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var act = () => service.LoginAsync(new LoginRequestDto { Email = "user@mail.com", Senha = "wrong" });

        await Assert.ThrowsAsync<BusinessRuleViolationException>(act);
        _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    private AuthService CreateService() =>
        new(_userRepositoryMock.Object, _passwordHasherMock.Object, _tokenServiceMock.Object);
}
