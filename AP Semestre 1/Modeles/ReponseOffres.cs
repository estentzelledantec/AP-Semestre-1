using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace AutoMasters.Modeles;

public class ReponseOffres
{
    #region Attributs

    private ObservableCollection<Offre> offres = new ObservableCollection<Offre>();

    #endregion

    #region Constructeurs

    #endregion

    #region Getter/Setter

    [JsonProperty("items")]
    public ObservableCollection<Offre> Offres
    {
        get { return offres; }
        set { offres = value; }
    }

    #endregion

    #region Methodes

    #endregion
}
