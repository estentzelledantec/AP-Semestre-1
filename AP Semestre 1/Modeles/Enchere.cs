namespace AutoMasters.Modeles;

public sealed class Enchere
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Carte Carte { get; set; } = new();
    public decimal PrixActuel { get; set; }
    public string Vendeur { get; set; } = "";
    public DateTimeOffset Fin { get; set; }
    public int NombreEncheres { get; set; }
}
