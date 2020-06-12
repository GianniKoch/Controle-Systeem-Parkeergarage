using Newtonsoft.Json;
using System.Collections.Generic;

namespace ControleSysteemParkeergarage.Models
{
    class Parkeerplaatsen
    {
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
