using System.Security.Cryptography;

namespace Kessler.Domain.Orders;

/// <summary>Gera códigos públicos curtos e legíveis, ex.: "KES-7FQ4K".</summary>
internal static class ReferenceCode
{
    // Sem 0/O/1/I para evitar confusão ao ditar o código.
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string New(string prefix)
    {
        var bytes = RandomNumberGenerator.GetBytes(5);
        var chars = new char[5];
        for (var i = 0; i < chars.Length; i++)
            chars[i] = Alphabet[bytes[i] % Alphabet.Length];
        return $"{prefix}-{new string(chars)}";
    }
}
