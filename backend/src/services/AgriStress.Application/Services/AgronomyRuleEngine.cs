using System.Text;
using AgriStress.Application.DTOs;
using AgriStress.Application.Interfaces;

namespace AgriStress.Application.Services;

public class AgronomyRuleEngine : IAgronomyRuleEngine
{
    public string GetImprovementRecipe(SoilAnalysisReport report)
    {
        if (report.Deficiencies == null || !report.Deficiencies.Any())
        {
            return "Toprağınız şu an çok sağlıklı görünüyor. Sadece rutin bakımlara devam edebilirsiniz.";
        }

        var recipe = new StringBuilder();
        recipe.AppendLine("Tespit edilen eksikliklere göre uzman tavsiyeleri:");

        bool needsNitrogen = report.Deficiencies.Any(d => d.Contains("Azot", StringComparison.OrdinalIgnoreCase));
        bool needsPhosphorus = report.Deficiencies.Any(d => d.Contains("Fosfor", StringComparison.OrdinalIgnoreCase));
        bool lowOrganicMatter = report.Deficiencies.Any(d => d.Contains("Organik Madde", StringComparison.OrdinalIgnoreCase));

        if (needsNitrogen)
        {
            recipe.AppendLine("- Azot eksikliği için: Ekim öncesi veya bitki büyüme döneminde dekara 15-20 kg Üre gübresi veya Amonyum Sülfat uygulamanız tavsiye edilir.");
        }

        if (needsPhosphorus)
        {
            recipe.AppendLine("- Fosfor eksikliği için: Kök gelişimini desteklemek adına ekimle birlikte (banda) dekara 15-20 kg DAP gübresi kullanmalısınız.");
        }

        if (lowOrganicMatter)
        {
            recipe.AppendLine("- Düşük Organik Madde için: Toprağın su tutma kapasitesini artırmak için sonbaharda dekara 3-4 ton iyi yanmış çiftlik gübresi veya leonardit bazlı organik düzenleyiciler uygulayın.");
        }

        if (!needsNitrogen && !needsPhosphorus && !lowOrganicMatter)
        {
            recipe.AppendLine("- Belirtilen diğer mikro element eksiklikleri için yapraktan sıvı gübre takviyesi yapabilirsiniz.");
        }

        return recipe.ToString();
    }
}
