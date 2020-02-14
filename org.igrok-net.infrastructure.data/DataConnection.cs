using MySql.Data.MySqlClient;
using org.igrok_net.infrastructure.domain.Interfaces;
using System;
using System.Data.Common;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;

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
        private SqlConnection _sqlServerConnection;
        private bool _mySqlMode = false;

        public DataConnection() : this(GetConnectionString())
        {
        }

        public DataConnection(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
            InitialiseDB();
        }


        public DataConnection(string connectionString, bool mySqlMode)
        {

            _mySqlMode = mySqlMode;
            if (mySqlMode)
            {
                _connection = new MySqlConnection(connectionString);
                _connection.Open();
                InitialiseDB();
            }
            else
            {
                _sqlServerConnection = new SqlConnection(connectionString);
                _sqlServerConnection.Open();
                InitialiseDB();
            }
        }

        private static string GetConnectionString()
        {
            var data = File.ReadAllText("localdb.json");
            var cfg = JsonConvert.DeserializeObject<Config>(data);
            return $"server={cfg.Host};user={cfg.User};database={cfg.DbName};port={cfg.Port};password={cfg.Password}";
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
            if(_sqlServerConnection != null)
            {
                _sqlServerConnection.Close();
            }
        }

        public void ExecuteNonQuery(string query)
        {
            try
            {
                if (_mySqlMode)
                {
                    var command = new MySqlCommand(query, _connection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    if (_sqlServerConnection != null)
                    {
                        var command = new SqlCommand(query, _sqlServerConnection);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }

        public DbDataReader ExecuteReader(string query)
        {
            try
            {
                if (_mySqlMode)
                {
                    var command = new MySqlCommand(query, _connection);
                    return command.ExecuteReader();
                }
                else
                {
                    if (_sqlServerConnection != null)
                    {
                        var command = new SqlCommand(query, _sqlServerConnection);
                        return command.ExecuteReader();
                    }
                    return null;
                }
            }
            catch(Exception e)
            {
                throw;
            }
        }

        public void InitialiseDB()
        {
            if (_mySqlMode)
            {
                InitialiseMysqlDB();
            }
            else
            {
                InitialiseMsSqlDB();
            }
        }

        private void InitialiseMsSqlDB()
        {
            var query = "if not exists(select * from sysobjects where name='licences' and xtype='U')" +
                "create table licences(" +
                "id bigint not null Identity(1,1), " +
                "licenceKey char(29) not null, " +
                "isUsed bit not null default 'false', " +
                "constraint pk_licenceId primary key(id)" +
                ");";
            query += "if not exists(select * from sysobjects where name='users' and xtype='U')" +
                "create table users(" +
                "id bigint not null Identity(1,1), " +
                "mail char(254) not null, " +
                "licence bigint null, constraint pk_userId primary key(id)," +
                ");";
            query += "if not exists(select * from sysobjects where name='telemetries' and xtype='U')" +
                "create table telemetries(" +
                "id bigint not null Identity(1,1), " +
                "osver varchar(1024) not null, " +
                "netfxver varchar(1024) not null, " +
                "userId bigint not null, constraint pk_telemetryId primary key(id)" +
                ");";
            query += "if not exists(select * from sysobjects where name='telemetryIps' and xtype='U')" +
                "create table telemetryIps(" +
                "id bigint not null Identity(1,1)," +
                "telemetryId bigint not null," +
                "ip char(254) not null," +
                "constraint pk_telemetryIps primary key(id)" +
                ");";
            var command = new SqlCommand(query, _sqlServerConnection);
            command.ExecuteNonQuery();
        }

        public void InitialiseMysqlDB()
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
