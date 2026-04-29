namespace Estoque.Application.DTOs.Users;

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string NomeCompleto { get; init; } = string.Empty;
    public int Idade { get; init; }
    public string Email { get; init; } = string.Empty;
}
