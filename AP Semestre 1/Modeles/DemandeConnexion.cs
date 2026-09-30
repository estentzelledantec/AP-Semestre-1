using Newtonsoft.Json;

namespace AutoMasters.Modeles;

public class DemandeConnexion
{
    #region Attributs

    private string identifiant = "";
    private string motDePasse = "";

    #endregion

    #region Constructeurs

    #endregion

    #region Getter/Setter

    [JsonProperty("username")]
    public string Identifiant
    {
        get { return identifiant; }
        set { identifiant = value; }
    }

    [JsonProperty("password")]
    public string MotDePasse
    {
        get { return motDePasse; }
        set { motDePasse = value; }
    }

    #endregion

    #region Methodes

    #endregion
}
