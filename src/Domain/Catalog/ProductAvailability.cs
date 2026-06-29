namespace Kessler.Domain.Catalog;

/// <summary>
/// Como a peça aparece no site. Um único modelo cobre vitrine, loja e sob encomenda.
/// </summary>
public enum ProductAvailability
{
    /// <summary>Só portfólio/galeria — não está à venda (sem preço/estoque).</summary>
    Showcase = 0,

    /// <summary>Peça pronta à venda — tem preço e estoque.</summary>
    ReadyToBuy = 1,

    /// <summary>Feita sob encomenda — preço "a partir de" e prazo de produção.</summary>
    MadeToOrder = 2
}
