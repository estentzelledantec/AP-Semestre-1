using Newtonsoft.Json;

namespace AutoMasters.Modeles;

public class ReponseConnexion
{
    #region Attributs

    private string jetonAcces = "";

    #endregion

    #region Constructeurs

    #endregion

    #region Getter/Setter

    [JsonProperty("accessToken")]
    public string JetonAcces
    {
        get { return jetonAcces; }
        set { jetonAcces = value; }
    }

    #endregion

    #region Methodes

    #endregion
}
