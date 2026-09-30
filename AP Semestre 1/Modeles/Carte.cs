using Newtonsoft.Json;

namespace AutoMasters.Modeles;

public class Carte
{
    #region Attributs

    private string identifiant = "";
    private string marque = "";
    private string modele = "";
    private string variante = "";

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

    [JsonProperty("brand")]
    public string Marque
    {
        get { return marque; }
        set { marque = value; }
    }

    [JsonProperty("model")]
    public string Modele
    {
        get { return modele; }
        set { modele = value; }
    }

    [JsonProperty("variant")]
    public string Variante
    {
        get { return variante; }
        set { variante = value; }
    }

    // Données d'une carte obtenue en jeu. Elles restent locales et ne font
    // pas partie du format échangé avec l'API des cartes possédées.
    [JsonIgnore]
    public Guid InstanceId { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public Vehicule Vehicule { get; set; } = new();

    [JsonIgnore]
    public DateTime ObtenueLe { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public decimal PrixAchat { get; set; }

    [JsonIgnore]
    public bool ALaVente { get; set; }

    #endregion

    #region Methodes

    #endregion
}
