using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace AutoMasters.Modeles;

public class Portefeuille
{
    #region Attributs

    private ObservableCollection<CartePossedee> cartesPossedees = new ObservableCollection<CartePossedee>();

    #endregion

    #region Constructeurs

    #endregion

    #region Getter/Setter

    [JsonProperty("positions")]
    public ObservableCollection<CartePossedee> CartesPossedees
    {
        get { return cartesPossedees; }
        set { cartesPossedees = value; }
    }

    #endregion

    #region Methodes

    #endregion
}
