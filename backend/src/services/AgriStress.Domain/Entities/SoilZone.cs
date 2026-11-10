using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriStress.Domain.Entities;

public class SoilZone
{
    public int Id { get; set; }
    
    // Geometri - CBS verilerinde standart olan MultiPolygon tercih edildi.
    public MultiPolygon? Boundary { get; set; }
    
    // 1. Makro Besin Elementleri
    [Column("topn")]
    public double? ToplamAzot { get; set; }
    
    [Column("topn_yet")]
    public string? AzotYeterlilik { get; set; }
    
    [Column("p205")]
    public double? Fosfor { get; set; }
    
    [Column("p205_yet")]
    public string? FosforYeterlilik { get; set; }
    
    [Column("k2o")]
    public double? Potasyum { get; set; }
    
    [Column("k2o_yet")]
    public string? PotasyumYeterlilik { get; set; }
    
    [Column("org_mad_y")]
    public double? OrganikMaddeYuzdesi { get; set; }

    // 2. Mikro Besin Elementleri
    [Column("fe_ppm")]
    public double? DemirPpm { get; set; }
    
    [Column("fe_yet")]
    public string? DemirYeterlilik { get; set; }
    
    [Column("zn_ppm")]
    public double? CinkoPpm { get; set; }
    
    [Column("zn_yet")]
    public string? CinkoYeterlilik { get; set; }
    
    [Column("mn_ppm")]
    public double? ManganPpm { get; set; }
    
    [Column("cu_ppm")]
    public double? BakirPpm { get; set; }
    
    [Column("b_ppm")]
    public double? BorPpm { get; set; }

    // 3. Fiziksel Doku & Arazi Jeomorfolojisi
    [Column("teks_ust")]
    public string? UstTekstur { get; set; }
    
    [Column("teks_alt")]
    public string? AltTekstur { get; set; }
    
    [Column("egim_sin")]
    public string? EgimSinifi { get; set; }
    
    [Column("der_sin")]
    public string? DerinlikSinifi { get; set; }
    
    [Column("akk_sin_t")]
    public string? AraziKullanimKabiliyeti { get; set; }

    // 4. Tarımsal Ürün Uygunluk
    [Column("smeyseb_aciklama")]
    public string? MeyveSebzeUygunlugu { get; set; }
    
    [Column("starla_aciklama")]
    public string? SuluTarlaUygunlugu { get; set; }
    
    [Column("ktarla_aciklama")]
    public string? KuruTarlaUygunlugu { get; set; }
}
