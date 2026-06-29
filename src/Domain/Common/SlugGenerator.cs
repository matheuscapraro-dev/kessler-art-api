using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kessler.Domain.Common;

/// <summary>
/// Gera slugs amigáveis para URL a partir de um texto livre — remove acentos,
/// baixa a caixa e troca tudo que não é alfanumérico por hífens.
/// Ex.: "Manta de Crochê Boho" → "manta-de-croche-boho".
/// </summary>
public static partial class SlugGenerator
{
    public static string Generate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove diacríticos (ç, ã, é …)
        var normalized = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);
        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC);
        slug = NonAlphanumeric().Replace(slug, "-"); // tudo que não é a-z/0-9 vira hífen
        slug = MultipleHyphens().Replace(slug, "-");  // colapsa hifens repetidos
        return slug.Trim('-');
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphanumeric();

    [GeneratedRegex("-{2,}")]
    private static partial Regex MultipleHyphens();
}
