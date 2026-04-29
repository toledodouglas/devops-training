namespace Estoque.Application.DTOs.Users;

public sealed class UserCreateDto
{
    public string NomeCompleto { get; set; } = string.Empty;
    public int Idade { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
