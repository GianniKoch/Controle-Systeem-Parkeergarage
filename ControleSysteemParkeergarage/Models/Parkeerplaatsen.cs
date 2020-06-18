using Newtonsoft.Json;
using System.Collections.Generic;

namespace ControleSysteemParkeergarage.Models
{

    //Model voor het inkomende JSON Bericht (key: parkeerplaats)
    class Parkeerplaatsen
    {

        //Properties in het JSON bericht onder de key parkeerplaats.
        public int id { get; set; }
        public bool B { get; set; }
        public bool H { get; set; }

        [JsonConstructor]
        public Parkeerplaatsen(int id, bool b, bool h)
        {
            this.id = id;
            B = b;
            H = h;
        }
        
        public static Parkeerplaatsen getParkeerplaatsFromId(List<Parkeerplaatsen> parkeerplaatsen, int id)
        {
            foreach (var plaats in parkeerplaatsen)
            {
                if (id.Equals(plaats.id))
                {
                    return plaats;
                }
            }
            return null;
        }
    }
}
