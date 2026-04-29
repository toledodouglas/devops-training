using Estoque.Application.Abstractions.Security;
using Estoque.Application.DTOs.Users;
using Estoque.Application.Services.Interfaces;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Repositories;

namespace Estoque.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> CreateAsync(UserCreateDto request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new BusinessRuleViolationException("E-mail já está cadastrado.");
        }

        if (request.Idade < 18)
        {
            throw new BusinessRuleViolationException("Cadastro permitido apenas para maiores de 18 anos.");
        }

        var user = new User
        {
            NomeCompleto = request.NomeCompleto,
            Idade = request.Idade,
            Email = request.Email.Trim().ToLowerInvariant(),
            SenhaHash = _passwordHasher.Hash(request.Senha)
        };

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            NomeCompleto = user.NomeCompleto,
            Idade = user.Idade,
            Email = user.Email
        };
    }
}
