using Microsoft.AspNetCore.Mvc;
using org.igrok_net.infrastructure.domain;
using org.igrok_net.infrastructure.domain.Interfaces;
using org.igrok_net.infrastructure.domain.Services;
using org.igrok_net.telemetry.Models;
using System;
using System.Linq;

namespace org.igrok_net.telemetry.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ServiceProvider _serviceProvider;
        private IDataAccess _dataConnection;

        public UserController(ServiceProvider serviceProvider, IDataAccess dataConn)
        {
            _serviceProvider = serviceProvider;
            _dataConnection = dataConn;
        }

        [HttpGet("request")]
        public IActionResult GetOrCreateUser([FromQuery]string email)
        {
            try
            {
                _serviceProvider.GetUserService().GetOrCreateUser(email);
                var user = _serviceProvider.GetUserService().GetUser(email);
                var result = new UserModel
                {
                    Id = user.Id,
                    Mail = user.Mail,
                    Licence = new LicenceModel()
                };
                LicenceKey licence;
                if (!user.LicenceKeyId.HasValue)
                {
                    if (_serviceProvider.GetLicenceService().GetFirstUnusedLicenceKey() == null)
                    {
                        var licenceId = _serviceProvider.GetLicenceService().GenerateLicence();
                        _serviceProvider.GetUserService().AssignLicence(user.Id, licenceId);
                        _serviceProvider.GetLicenceService().SetUsed(licenceId);
                        user = _serviceProvider.GetUserService().GetUser(email);
                    }
                }
                if (user.LicenceKeyId.HasValue)
                {
                    licence = _serviceProvider.GetLicenceService().GetLicenceKey(user.LicenceKeyId.Value);
                    result.Licence = new LicenceModel
                    {
                        Id = licence.Id,
                        IsUsed = licence.IsUsed,
                        Key = licence.Key
                    };
                }
                TelemetryRecord telemetry = _serviceProvider.GetTelemetryService().GetTelemetryRecordFor(user.Id);
                if (telemetry != null)
                {
                    result.Telemetry = new TelemetryModel
                    {
                        Email = user.Mail,
                        NetFxVersion = telemetry.NetFxVersion,
                        OsVersion = telemetry.OsVersion
                    };
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("sendtdata")]
        [Consumes("application/json")]
        public IActionResult CreateTelemetry([FromBody]TelemetryModel telemetry)
        {
            try
            {
                if (telemetry == null)
                {
                    return BadRequest("Telemetry record is empty");
                }
                var user = _serviceProvider.GetUserService().GetUser(telemetry.Email);
                if (user == null || !user.LicenceKeyId.HasValue)
                {
                    return NotFound("User not found or inactive");
                }
                var telemetryRecord = _serviceProvider.GetTelemetryService().CreateOrUpdateTelemetryRecord(user.Id, telemetry.OsVersion, telemetry.NetFxVersion);
                var clientIp = Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? Request.Headers["X-Forwarded-For"].FirstOrDefault();
                var resultReader = _dataConnection.ExecuteReader($"SELECT COUNT(*) FROM telemetryIps WHERE telemetryId = {user.Id} AND ip = \"{clientIp}\"");
                if (resultReader.HasRows)
                {
                    resultReader.Read();
                    if (resultReader.GetInt32(0) > 0)
                    {
                        _dataConnection.ExecuteNonQuery($"INSERT INTO telemetryIps(telemetryId,ip) VALUES({telemetryRecord},\"{clientIp}\")");
                    }
                    resultReader.Close();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}