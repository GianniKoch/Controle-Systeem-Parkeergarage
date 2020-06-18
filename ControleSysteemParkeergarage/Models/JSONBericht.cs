using System.Collections.Generic;

namespace ControleSysteemParkeergarage.Models
{
    //Model voor het inkomende JSON Bericht.

    class JSONBericht
    {
        public List<Parkeerplaatsen> ParkeerPlaatsen { get; set; }
        public int AantalAutosInParkeerplaats { get; set; }

        public JSONBericht(List<Parkeerplaatsen> parkeerPlaatsen, int aantalAutosInParkeerplaats)
        {
            //Properties een waarde geven van het inkomende bericht.
            ParkeerPlaatsen = parkeerPlaatsen;
            AantalAutosInParkeerplaats = aantalAutosInParkeerplaats;
        }

        public JSONBericht()
        {
            ParkeerPlaatsen = new List<Parkeerplaatsen>();
        }
    }
}
