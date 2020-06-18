using System;
using System.Collections.Generic;

namespace ControleSysteemParkeergarage
{
    class Business
    {
        //Globale variabele van de vervinding met de database klasse.
        private Persistance _persistance;

        public Business(string[] MySqlSettings) {
            //Gegevens van de database megeven met initialisatie van de persistance klasse.
            _persistance = new Persistance(MySqlSettings[0], MySqlSettings[1], MySqlSettings[2], MySqlSettings[3]);
            
            //Als de connectie gelukt is dit loggen.
            if(_persistance.ConnectionSuccess) 
                log(Persistance.logType.ConnectionSucces, "Connectie gelukt!", DateTime.Now);

            //Propertie instellen.
            ConnectionOpen = _persistance.ConnectionSuccess;
        }

        public bool ConnectionOpen { get; }

        public bool log(Persistance.logType type, string value, DateTime dt)
        {
            //functie doorsturen naar de persistance.
            return _persistance.log(type, value, dt);
        }
    }
}
