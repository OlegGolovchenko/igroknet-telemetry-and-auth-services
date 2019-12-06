using MySql.Data.MySqlClient;
using org.igrok_net.infrastructure.domain.Interfaces;
using System;
using System.Data.Common;
using System.IO;
using Newtonsoft.Json;

namespace org.igrok_net.infrastructure.data
{
    public class Config
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string DbName { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }

    public class DataConnection : IDataAccess, IDisposable
    {
        private MySqlConnection _connection;

        public DataConnection():this(GetConnectionString())
        {
        }

        public DataConnection(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
            InitialiseDB();
        }
        private static string GetConnectionString()
        {
            var data = File.ReadAllText("localdb.json");
            var cfg = JsonConvert.DeserializeObject<Config>(data);
            return $"server={cfg.Host};user={cfg.User};database={cfg.DbName};port={cfg.Port};password={cfg.Password}";
        }

        public void Dispose()
        {
            if(_connection != null)
            {
                _connection.Close();
            }
        }

        public void ExecuteNonQuery(string query)
        {
            var command = new MySqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(string query)
        {
            var command = new MySqlCommand(query, _connection);
            return command.ExecuteReader();
        }

        public void InitialiseDB()
        {
            var query = "create table if not exists licences(" +
                "id bigint not null auto_increment, " +
                "licenceKey char(29) character set utf8mb4 not null, " +
                "isUsed bit not null default false, " +
                "constraint pk_licenceId primary key(id)" +
                ");";
            query += "create table if not exists users(" +
                "id bigint not null auto_increment, " +
                "mail char(254) character set utf8mb4 not null, " +
                "licence bigint null, constraint pk_userId primary key(id)," +
                "constraint fk_users_licence foreign key(licence) references licences(id)" +
                ");";
            query += "create table if not exists telemetries(" +
                "id bigint not null auto_increment, " +
                "osver varchar(1024) character set utf8mb4 not null, " +
                "netfxver varchar(1024) character set utf8mb4 not null, " +
                "userId bigint not null, constraint pk_telemetryId primary key(id)" +
                ");";
            query += "create table if not exists telemetryIps(" +
                "id bigint not null auto_increment," +
                "telemetryId bigint not null," +
                "ip char(254) character set utf8mb4 not null," +
                "constraint pk_telemetryIps primary key(id)" +
                ");";
            var command = new MySqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }
    }
}
