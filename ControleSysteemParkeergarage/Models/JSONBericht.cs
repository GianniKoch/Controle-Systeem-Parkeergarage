using System.Collections.Generic;

namespace ControleSysteemParkeergarage.Models
{
    class JSONBericht
    {
        public List<Parkeerplaatsen> ParkeerPlaatsen { get; set; }
        public int AantalAutosInParkeerplaats { get; set; }

        public JSONBericht(List<Parkeerplaatsen> parkeerPlaatsen, int aantalAutosInParkeerplaats)
        {
            ParkeerPlaatsen = parkeerPlaatsen;
            AantalAutosInParkeerplaats = aantalAutosInParkeerplaats;
        }

        public JSONBericht()
        {
            ParkeerPlaatsen = new List<Parkeerplaatsen>();
        }
    }
}
