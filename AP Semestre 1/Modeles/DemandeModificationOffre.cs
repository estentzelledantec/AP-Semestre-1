using Newtonsoft.Json;

namespace AutoMasters.Modeles;

public class DemandeModificationOffre
{
    #region Attributs

    private decimal prix = 0;

    #endregion

    #region Constructeurs

    #endregion

    #region Getter/Setter

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
