using Estoque.Application.Abstractions.Security;
using Estoque.Application.DTOs.Auth;
using Estoque.Application.Services.Interfaces;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Repositories;

namespace Estoque.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Senha, user.SenhaHash))
        {
            throw new BusinessRuleViolationException("Credenciais inválidas.");
        }

        return new LoginResponseDto
        {
            AccessToken = _tokenService.GenerateToken(user)
        };
    }
}
