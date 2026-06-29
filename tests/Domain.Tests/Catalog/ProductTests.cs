using Kessler.Domain.Catalog;
using Kessler.Domain.Common.Errors;

namespace Domain.Tests.Catalog;

public class ProductTests
{
    private static readonly Guid CategoryId = Guid.NewGuid();

    [Fact]
    public void Create_ReadyToBuy_WithoutPrice_Throws()
    {
        var act = () => Product.Create("Manta", CategoryId, ProductAvailability.ReadyToBuy);
        Assert.Throws<DomainException>(act);
    }

    [Fact]
    public void Create_Showcase_ClearsPriceAndStock()
    {
        var product = Product.Create("Coelho", CategoryId, ProductAvailability.Showcase, price: 100, stockQuantity: 5);

        Assert.Null(product.Price);
        Assert.Null(product.StockQuantity);
        Assert.Equal("coelho", product.Slug);
    }

    [Fact]
    public void Create_ReadyToBuy_DefaultsStockToZero()
    {
        var product = Product.Create("Amigurumi", CategoryId, ProductAvailability.ReadyToBuy, price: 80);

        Assert.Equal(80, product.Price);
        Assert.Equal(0, product.StockQuantity);
    }

    [Fact]
    public void AddImage_FirstBecomesCover()
    {
        var product = Product.Create("Peça", CategoryId, ProductAvailability.Showcase);

        var first = product.AddImage("k1", "u1");
        var second = product.AddImage("k2", "u2");

        Assert.True(first.IsCover);
        Assert.False(second.IsCover);
    }

    [Fact]
    public void RemoveCover_PromotesNextImage()
    {
        var product = Product.Create("Peça", CategoryId, ProductAvailability.Showcase);
        var first = product.AddImage("k1", "u1");
        product.AddImage("k2", "u2");

        product.RemoveImage(first.Id);

        Assert.True(product.Images[0].IsCover);
        Assert.Single(product.Images);
    }

    [Fact]
    public void DecreaseStock_BelowZero_Throws()
    {
        var product = Product.Create("Pronta", CategoryId, ProductAvailability.ReadyToBuy, price: 50, stockQuantity: 1);

        product.DecreaseStock(1);
        Assert.Equal(0, product.StockQuantity);
        Assert.Throws<DomainException>(() => product.DecreaseStock(1));
    }
}
