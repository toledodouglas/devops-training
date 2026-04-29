using Estoque.Domain.Entities;

namespace Estoque.Application.Abstractions.Security;

public interface ITokenService
{
    string GenerateToken(User user);
}
