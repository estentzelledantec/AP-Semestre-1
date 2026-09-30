using Newtonsoft.Json;

namespace AutoMasters.Modeles;

public class Offre
{
    #region Attributs

    private string identifiant = "";
    private string identifiantExemplaire = "";
    private decimal prix = 0;
    private string statut = "";

    #endregion

    #region Constructeurs

    #endregion

    #region Getter/Setter

    [JsonProperty("id")]
    public string Identifiant
    {
        get { return identifiant; }
        set { identifiant = value; }
    }

    [JsonProperty("copyId")]
    public string IdentifiantExemplaire
    {
        get { return identifiantExemplaire; }
        set { identifiantExemplaire = value; }
    }

    [JsonProperty("price")]
    public decimal Prix
    {
        get { return prix; }
        set { prix = value; }
    }

    [JsonProperty("status")]
    public string Statut
    {
        get { return statut; }
        set { statut = value; }
    }

    #endregion

    #region Methodes

    #endregion
}
