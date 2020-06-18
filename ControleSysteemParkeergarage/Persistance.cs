using System;
using System.Collections.Generic; //Dictionary<,> / List<>
using MySql.Data.MySqlClient; //MySqlConnection / command

namespace ControleSysteemParkeergarage
{
    class Persistance
    {
        //Globale variabelen
        private MySqlConnection _connection;
        private MySqlCommand _command;
        private Dictionary<logType, int> _dbTypeIds;
        public bool ConnectionSuccess { get; }

        public Persistance(string localhost, string user, string password, string database)
        {
            try
            {
                //Connectie maken.
                _connection = new MySqlConnection($@"server={localhost};user id={user};password={password};database={database}");
                _connection.Open();
                
                //Propertie instellen op de connectie state.
                ConnectionSuccess = _connection.State.ToString() == "Open";
                _connection.Close();
            }
            catch (Exception)
            {
                ConnectionSuccess = false;
                _connection.Close();
            }
            _dbTypeIds = getTypeIdsFromDb();
        }

        public Dictionary<logType, int> getTypeIdsFromDb()
        {
            try
            {
                //Connectie opstellen.
                _connection.Open();
                _command = _connection.CreateCommand();
                var query = $"SELECT * FROM controleparkeergaragelogging.logtypes;";
                _command.CommandText = query;

                //Connectie uitvoeren en opslagen in een reader.
                var reader = _command.ExecuteReader();
                var dbTypeIds = new Dictionary<logType, int>();
                while (reader.Read())
                {
                    //Alle types toevoegen aan de dictionary.
                    dbTypeIds.Add((logType)Enum.Parse(typeof(logType), reader["typename"].ToString()),
                        int.Parse(reader["idlogtypes"].ToString()));
                }
                _connection.Close();
                return dbTypeIds;
            }
            catch (Exception)
            {
                _connection.Close();
                return new Dictionary<logType, int>(); 
            }
        }

        public int getIdFromLogType(logType type)
        {
            //Id terug sturen waarvan de enum overeenkomt in de dictionary.
            int id;
            if (_dbTypeIds.TryGetValue(type, out id))
            {
                return id;
            }
            return -1;
        }

        public bool log(logType type, string val, DateTime time)
        {
            try
            {
                //Query opstellen.
                var succes = 0;
                _connection.Open();
                _command = _connection.CreateCommand();
                var query = $"insert into logs (logtype, value, time) values ('{getIdFromLogType(type)}','{val}','{time.ToString("yyyy-MM-dd HH:mm:ss")}')";
                _command.CommandText = query;
                //Connectie uitvoeren.
                succes = _command.ExecuteNonQuery();
                _connection.Close();
                //Terug sturen of het is gelukt of niet.
                if (succes > 0)
                    return true;
                return false;
            }
            catch (Exception)
            {
                _connection.Close();
                return false;
            }
        }

        public enum logType
        {
            //Enum waarden om de string te vermijden.
            AutoIngaand,
            AutoUitgaand,
            ParkeerplaatsBezet,
            ParkeerplaatsVrij,
            ParkeergarageVol,
            ParkeergarageLeeg,
            ConnectionSucces
        }
    }
}
