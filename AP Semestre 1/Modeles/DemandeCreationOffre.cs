using Newtonsoft.Json;

namespace AutoMasters.Modeles;

public class DemandeCreationOffre
{
    #region Attributs

    private string identifiantExemplaire = "";
    private decimal prix = 0;

    #endregion

    #region Constructeurs

    #endregion

    #region Getter/Setter

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

    #endregion

    #region Methodes

    #endregion
}
