using org.igrok_net.infrastructure.domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace org.igrok_net.infrastructure.domain.Services
{
    class LicenceService : ILicenceService
    {
        private IDataAccess _dataAccess;
        private List<LicenceKey> _localCache;

        public LicenceService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _localCache = new List<LicenceKey>();
        }

        public long GenerateLicence()
        {
            var licenceKey = new LicenceKey();
            _dataAccess.ExecuteNonQuery($"INSERT INTO licences(licenceKey) VALUES('{licenceKey.Key}');");
            ActualiseCache();
            return _localCache.Where(x => x.Key == licenceKey.Key).Select(x => x.Id).SingleOrDefault();
        }

        public LicenceKey GetLicenceKey(long licenceId)
        {
            ActualiseCache();
            return _localCache.SingleOrDefault(x => x.Id == licenceId);
        }

        private void ActualiseCache()
        {
            _localCache.Clear();
            var reader = _dataAccess.ExecuteReader("SELECT id, licenceKey, isUsed FROM licences;");
            if (reader.HasRows)
            {
                reader.Read();
                var licenceKey = new LicenceKey(reader.GetInt64(0), reader.GetString(1), reader.GetBoolean(2));
                if (!_localCache.Contains(licenceKey))
                {
                    _localCache.Add(licenceKey);
                }
                while (reader.Read())
                {
                    licenceKey = new LicenceKey(reader.GetInt64(0), reader.GetString(1), reader.GetBoolean(2));
                    if (!_localCache.Contains(licenceKey))
                    {
                        _localCache.Add(licenceKey);
                    }
                }
            }
            reader.Close();
        }

        public void SetUnused(long licenceId)
        {
            ActualiseCache();
            if (_localCache.Any(x=>x.Id == licenceId))
            {
                _dataAccess.ExecuteNonQuery($"UPDATE licences SET isUsed = 0 WHERE Id = {licenceId};");
            }
            ActualiseCache();
        }

        public void SetUsed(long licenceId)
        {
            ActualiseCache();
            if (_localCache.Any(x => x.Id == licenceId))
            {
                _dataAccess.ExecuteNonQuery($"UPDATE licences SET isUsed = 1 WHERE Id = {licenceId};");
            }
            ActualiseCache();
        }

        public LicenceKey GetFirstUnusedLicenceKey()
        {
            ActualiseCache();
            return _localCache.FirstOrDefault(x=>!x.IsUsed);
        }
    }
}
