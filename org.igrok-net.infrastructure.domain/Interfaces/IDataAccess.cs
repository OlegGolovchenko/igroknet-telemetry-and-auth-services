using System.Data.Common;

namespace org.igrok_net.infrastructure.domain.Interfaces
{
    public interface IDataAccess
    {
        DbDataReader ExecuteReader(string query);
        void ExecuteNonQuery(string query);
        void InitialiseDB();
    }
}
