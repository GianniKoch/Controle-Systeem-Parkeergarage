using System;
using System.Collections.Generic;

namespace ControleSysteemParkeergarage
{
    class Business
    {
        private Persistance _persistance;

        public Business(string[] MySqlSettings) {
            _persistance = new Persistance(MySqlSettings[0], MySqlSettings[1], MySqlSettings[2], MySqlSettings[3]);
            if(_persistance.ConnectionSuccess) 
                log(Persistance.logType.ConnectionSucces, "Connectie gelukt!", DateTime.Now);
            ConnectionOpen = _persistance.ConnectionSuccess;
        }

        public bool ConnectionOpen { get; }

        public bool log(Persistance.logType type, string value, DateTime dt)
        {
            return _persistance.log(type, value, dt);
        }
    }
}
