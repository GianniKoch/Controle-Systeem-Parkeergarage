using System;
using System.Collections.Generic;

namespace ControleSysteemParkeergarage
{
    class Business
    {
        public string[] MySqlSettings { get; set; }

        private Persistance _persistance;

        public Business() {
            _persistance = new Persistance()
            {
                localhost = MySqlSettings[0],
                user = MySqlSettings[1],
                password = MySqlSettings[2],
                database = MySqlSettings[3]
            };
        }

        public bool log(Persistance.logType type, string value, DateTime dt)
        {
            _persistance.log(type, value, dt);
            return false;
        }
    }
}
