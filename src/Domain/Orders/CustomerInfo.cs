namespace Kessler.Domain.Orders;

/// <summary>
/// Dados de contato do cliente (compra/encomenda como convidado — sem conta).
/// Value object embutido (owned) em Order e CommissionRequest.
/// </summary>
public sealed record CustomerInfo(string Name, string Email, string Phone)
{
    public static CustomerInfo Create(string name, string email, string phone) =>
        new(name.Trim(), email.Trim().ToLowerInvariant(), phone.Trim());
}
