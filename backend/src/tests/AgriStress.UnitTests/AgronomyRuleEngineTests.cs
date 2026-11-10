using AgriStress.Application.DTOs;
using AgriStress.Application.Services;
using Xunit;

namespace AgriStress.UnitTests;

public class AgronomyRuleEngineTests
{
    private readonly AgronomyRuleEngine _ruleEngine = new();

    [Fact]
    public void GetImprovementRecipe_ShouldRecommendUrea_WhenNitrogenIsDeficient()
    {
        // Arrange
        var report = new SoilAnalysisReport
        {
            Deficiencies = new List<string> { "Azot Eksikliği (N) - Kök ve Gövde gelişimini yavaşlatır." }
        };

        // Act
        var recipe = _ruleEngine.GetImprovementRecipe(report);

        // Assert
        Assert.Contains("Üre gübresi", recipe);
    }

    [Fact]
    public void GetImprovementRecipe_ShouldRecommendDAP_WhenPhosphorusIsDeficient()
    {
        // Arrange
        var report = new SoilAnalysisReport
        {
            Deficiencies = new List<string> { "Fosfor Eksikliği (P) - Çiçeklenme ve tohum tutumunu olumsuz etkiler." }
        };

        // Act
        var recipe = _ruleEngine.GetImprovementRecipe(report);

        // Assert
        Assert.Contains("DAP gübresi", recipe);
    }

    [Fact]
    public void GetImprovementRecipe_ShouldReturnHealthyMessage_WhenNoDeficienciesExist()
    {
        // Arrange
        var report = new SoilAnalysisReport
        {
            Deficiencies = new List<string>()
        };

        // Act
        var recipe = _ruleEngine.GetImprovementRecipe(report);

        // Assert
        Assert.Contains("Toprağınız şu an çok sağlıklı görünüyor", recipe);
    }
}
