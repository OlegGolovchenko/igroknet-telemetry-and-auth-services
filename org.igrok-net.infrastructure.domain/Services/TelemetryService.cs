using org.igrok_net.infrastructure.domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace org.igrok_net.infrastructure.domain.Services
{
    class TelemetryService : ITelemetryService
    {

        private IDataAccess _dataAccess;
        private List<TelemetryRecord> _localCache;

        public TelemetryService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _localCache = new List<TelemetryRecord>();
        }

        private void ActualiseCache()
        {
            _localCache.Clear();
            var reader = _dataAccess.ExecuteReader("SELECT id, osver, netfxver, userId FROM telemetries;");
            if (reader.HasRows)
            {
                reader.Read();
                var tr = new TelemetryRecord(reader.GetInt64(0), reader.GetString(1), reader.GetString(2), reader.GetInt64(3));
                if (!_localCache.Contains(tr))
                {
                    _localCache.Add(tr);
                }
                while (reader.Read())
                {
                    tr = new TelemetryRecord(reader.GetInt64(0), reader.GetString(1), reader.GetString(2), reader.GetInt64(3));
                    if (!_localCache.Contains(tr))
                    {
                        _localCache.Add(tr);
                    }
                }
            }
            reader.Close();
        }

        public long CreateOrUpdateTelemetryRecord(long userId, string osName, string netFxVersion)
        {
            var tr = GetTelemetryRecordFor(userId);
            if(tr == null)
            {
                _dataAccess.ExecuteNonQuery($"INSERT INTO telemetries(userId, osver, netfxver) VALUES({userId}, '{osName}', '{netFxVersion}');");
            }
            else
            {
                _dataAccess.ExecuteNonQuery($"UPDATE telemetries SET osver = '{osName}' WHERE id = {userId};");
                _dataAccess.ExecuteNonQuery($"UPDATE telemetries SET netfxver = '{netFxVersion}' WHERE id = {userId};");
            }
            tr = GetTelemetryRecordFor(userId);
            return tr.Id;
        }

        public TelemetryRecord GetTelemetryRecord(long recordId)
        {
            ActualiseCache();
            return _localCache.SingleOrDefault(x => x.Id == recordId);
        }

        public TelemetryRecord GetTelemetryRecordFor(long userId)
        {
            ActualiseCache();
            return _localCache.SingleOrDefault(x => x.UserId == userId);
        }

        public bool RecordExists(long userId)
        {
            ActualiseCache();
            return _localCache.Any(x => x.UserId == userId);
        }

        public void RemoveTelemetryRecord(long recordId)
        {
            if (_localCache.Any(x => x.Id == recordId))
            {
                _dataAccess.ExecuteNonQuery($"DELETE FROM telemetries WHERE id={recordId};");
            }
            ActualiseCache();
        }
    }
}
