using AgriStress.Application.DTOs;

namespace AgriStress.Application.Interfaces;

public interface IAgronomyRuleEngine
{
    string GetImprovementRecipe(SoilAnalysisReport report);
}
