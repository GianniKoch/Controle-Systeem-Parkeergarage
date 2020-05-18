using Newtonsoft.Json;

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
    }
}
