namespace AutoMasters.Modeles;
public sealed class Vehicule
{
    public int Id { get; set; }
    public string Marque { get; set; } = "";
    public string Modele { get; set; } = "";
    public string Motorisation { get; set; } = "";
    public int Annee { get; set; }
    public int PuissanceCh { get; set; }
    public int Production { get; set; }
    public decimal Prix { get; set; }
    public Rarete Rarete { get; set; }
    public string ImageUrl { get; set; } = "";
    public string Description { get; set; } = "";
}
