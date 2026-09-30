namespace AutoMasters.Modeles;

public sealed class Booster
{
    public int Stock { get; set; }
    public int StockMax { get; set; } = 10;
    public DateTimeOffset? ProchainDisponible { get; set; }
    public List<Carte> Cartes { get; set; } = [];
}
