using org.igrok_net.infrastructure.domain.Interfaces;
using org.igrok_net.infrastructure.domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace org.igrok_net.infrastructure.domain.Services
{
    class UserService : IUserService
    {
        private IDataAccess _dataAccess;
        private List<User> _localCache;

        public UserService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _localCache = new List<User>();
        }

        public void AssignLicence(long userId,long licenceId)
        {
            var user = _localCache.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not exist.");
            }
            _dataAccess.ExecuteNonQuery($"UPDATE users SET licence = {licenceId} WHERE id = {userId};");
            ActualiseCache();
        }

        public long GetOrCreateUser(string email)
        {

            var user = GetUser(email);
            if (user == null)
            {
                if (_dataAccess != null)
                {
                    _dataAccess.ExecuteNonQuery($"INSERT INTO users(mail) VALUES('{email}');");
                }
            }
            user = GetUser(email);
            if (user != null)
            {
                return user.Id;
            }
            return -1;
        }

        public User GetUser(string email)
        {
            ActualiseCache();
            if (_localCache != null)
            {
                return _localCache.SingleOrDefault(x => x.Mail == email);
            }
            return null;
        }

        public void RemoveUser(long userId)
        {
            if(_localCache.Any(x=>x.Id == userId))
            {
                _dataAccess.ExecuteNonQuery($"DELETE FROM users WHERE id={userId};");
            }
            ActualiseCache();
        }

        public void ResignLicence(long userId)
        {
            var user = _localCache.SingleOrDefault(x => x.Id == userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not exist.");
            }
            _dataAccess.ExecuteNonQuery($"UPDATE users SET licence = NULL WHERE id = {userId};");
            ActualiseCache();
        }

        public bool UserExists(string email)
        {
            ActualiseCache();
            return _localCache.Any(x => x.Mail == email);
        }

        public bool UserExists(long userId)
        {
            ActualiseCache();
            return _localCache.Any(x => x.Id == userId);
        }

        public List<UserListModel> ListUsers()
        {
            ActualiseCache();
            return _localCache.Select(usr => new UserListModel
            {
                Id = usr.Id,
                Email = usr.Mail
            }).ToList();
        }

        private void ActualiseCache()
        {
            _localCache.Clear();
            var reader = _dataAccess.ExecuteReader("SELECT id, mail, licence FROM users;");
            if (reader.HasRows)
            {
                reader.Read();
                long? licenceId = null;
                if (!reader.IsDBNull(2))
                {
                    licenceId = reader.GetInt64(2);
                }
                var user = new User(reader.GetInt64(0), reader.GetString(1), licenceId);
                if (!_localCache.Contains(user))
                {
                    _localCache.Add(user);
                }
                while (reader.Read())
                {
                    licenceId = null;
                    if (!reader.IsDBNull(2))
                    {
                        licenceId = reader.GetInt64(2);
                    }
                    user = new User(reader.GetInt64(0), reader.GetString(1), licenceId);
                    if (!_localCache.Contains(user))
                    {
                        _localCache.Add(user);
                    }
                }
            }
            reader.Close();
        }
    }
}
