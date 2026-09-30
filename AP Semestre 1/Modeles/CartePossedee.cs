using Newtonsoft.Json;

namespace AutoMasters.Modeles;

public class CartePossedee
{
    #region Attributs

    private string identifiant = "";
    private Carte carte = new Carte();
    private decimal valeurActuelle = 0;

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

    [JsonProperty("card")]
    public Carte Carte
    {
        get { return carte; }
        set { carte = value; }
    }

    [JsonProperty("currentValue")]
    public decimal ValeurActuelle
    {
        get { return valeurActuelle; }
        set { valeurActuelle = value; }
    }

    #endregion

    #region Methodes

    #endregion
}
