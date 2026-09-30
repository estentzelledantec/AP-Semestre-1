using Newtonsoft.Json;

namespace AutoMasters.Modeles;

public class ErreurApi
{
    #region Attributs

    private string message = "La demande a échoué.";

    #endregion

    #region Constructeurs

    #endregion

    #region Getter/Setter

    [JsonProperty("message")]
    public string Message
    {
        get { return message; }
        set { message = value; }
    }

    #endregion

    #region Methodes

    #endregion
}
