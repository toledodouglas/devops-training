namespace Estoque.Domain.Exceptions;

public sealed class NegativeStockException : DomainException
{
    public NegativeStockException(string productName, int requested, int available)
        : base($"Estoque insuficiente para '{productName}'. Solicitado: {requested}, disponível: {available}.")
    {
    }
}
