using Kessler.Domain.Common;

namespace Domain.Tests.Common;

public class SlugGeneratorTests
{
    [Theory]
    [InlineData("Manta de Crochê Boho", "manta-de-croche-boho")]
    [InlineData("  Amigurumi  Coração ", "amigurumi-coracao")]
    [InlineData("Decoração & Cia!", "decoracao-cia")]
    [InlineData("Açaí", "acai")]
    public void Generate_NormalizesText(string input, string expected)
    {
        Assert.Equal(expected, SlugGenerator.Generate(input));
    }
}
