﻿using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ControleSysteemParkeergarage
{
    class Persistance
    {
        public string localhost { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string database { get; set; }

        private MySqlConnection _connection;
        private MySqlCommand _command;
        private Dictionary<logType, int> _dbTypeIds;

        public Persistance()
        {
            _connection = new MySqlConnection($@"server={localhost};user id={user};password={password};database={database}");
        }

        public Dictionary<logType, int> getTypeIdsFromDb()
        {
            try
            {
                _connection.Open();
                _command = _connection.CreateCommand();
                var query = $"select * from logtypes";
                _command.CommandText = query;
                var reader = _command.ExecuteReader();
                var _dbTypeIds = new Dictionary<logType, int>();
                while (reader.Read())
                {
                    _dbTypeIds.Add((logType)Enum.Parse(typeof(logType), reader["typename"].ToString()),
                        int.Parse(reader["idlogtypes"].ToString()));
                }
                _connection.Close();
                return _dbTypeIds;
            }
            catch (Exception) { return new Dictionary<logType, int>(); }
        }

        public int getIdFromLogType(logType type)
        {
            int id;
            if (_dbTypeIds.TryGetValue(type, out id))
            {
                return id;
            }
            return 0;
        }

        public bool log(logType type, string val, DateTime time)
        {
            try
            {
                var succes = 0;
                _connection.Open();
                _command = _connection.CreateCommand();
                var query = $"insert into logs (logtype, value, time) values ({getIdFromLogType(type)},{val},{time.ToString("yyyy-MM-dd HH:mm:ss")})";
                _command.CommandText = query;
                succes = _command.ExecuteNonQuery();
                _connection.Close();
                if (succes > 0)
                    return true;
                return false;
            }
            catch (Exception) { return false; }
        }

        public enum logType
        {
            AutoIngaand,
            AutoUitgaand,
            ParkeerplaatsBezet,
            ParkeerplaatsVrij,
            ParkeerpgarageVol,
            ParkeergarageLeeg,
            ConnectionSucces
        }
    }
}