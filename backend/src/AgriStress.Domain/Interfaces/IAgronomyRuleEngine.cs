using AgriStress.Domain.Models;

namespace AgriStress.Domain.Interfaces;

public interface IAgronomyRuleEngine
{
    string GetImprovementRecipe(SoilAnalysisReport report);
}
