using Microsoft.AspNetCore.Mvc;
using org.igrok_net.infrastructure.domain;
using org.igrok_net.infrastructure.domain.Services;
using org.igrok_net.telemetry.Models;

namespace org.igrok_net.telemetry.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private ServiceProvider _serviceProvider;

        public UserController(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet("request")]
        public IActionResult GetOrCreateUser([FromQuery]string email)
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
                var licenceId = _serviceProvider.GetLicenceService().GenerateLicence();
                _serviceProvider.GetUserService().AssignLicence(user.Id,licenceId);
                _serviceProvider.GetLicenceService().SetUsed(licenceId);
                user = _serviceProvider.GetUserService().GetUser(email);
            }
            licence = _serviceProvider.GetLicenceService().GetLicenceKey(user.LicenceKeyId.Value);
            result.Licence = new LicenceModel
            {
                Id = licence.Id,
                IsUsed = licence.IsUsed,
                Key = licence.Key
            };
            TelemetryRecord telemetry = _serviceProvider.GetTelemetryService().GetTelemetryRecordFor(user.Id);
            if(telemetry != null)
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

        [HttpPost("sendtdata")]
        [Consumes("application/json")]
        public IActionResult CreateTelemetry([FromBody]TelemetryModel telemetry)
        {
            if(telemetry == null)
            {
                return BadRequest("Telemetry record is empty");
            }
            var user = _serviceProvider.GetUserService().GetUser(telemetry.Email);
            if(user == null || !user.LicenceKeyId.HasValue)
            {
                return NotFound("User not found or inactive");
            }
            var telemetryRecord = _serviceProvider.GetTelemetryService().CreateOrUpdateTelemetryRecord(user.Id, telemetry.OsVersion, telemetry.NetFxVersion);
            return Ok();
        }
    }
}