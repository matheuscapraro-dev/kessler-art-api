using Kessler.Domain.Common;

namespace Kessler.Domain.Catalog;

/// <summary>
/// Categoria do catálogo (ex.: Amigurumis, Mantas, Decoração). Usada para
/// agrupar/filtrar peças na galeria e na loja.
/// </summary>
public sealed class Category : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsPublished { get; private set; }

    private Category() { } // EF Core

    public static Category Create(string name, string? description = null, int displayOrder = 0)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = SlugGenerator.Generate(name),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            DisplayOrder = displayOrder,
            IsPublished = true
        };
    }

    public void Update(string name, string? description, int displayOrder, bool isPublished)
    {
        Name = name.Trim();
        Slug = SlugGenerator.Generate(name);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        DisplayOrder = displayOrder;
        IsPublished = isPublished;
    }
}
