using Estoque.Application.DTOs.Users;

namespace Estoque.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateAsync(UserCreateDto request, CancellationToken cancellationToken = default);
}
