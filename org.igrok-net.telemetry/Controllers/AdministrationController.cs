﻿using Microsoft.AspNetCore.Mvc;
using org.igrok_net.infrastructure.data;
using org.igrok_net.infrastructure.domain;
using org.igrok_net.infrastructure.domain.Services;
using org.igrok_net.telemetry.Models;

namespace org.igrok_net.telemetry.Controllers
{
    [Route("api/adm")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private ServiceProvider _serviceProvider;
        private AdminAccessCode _adminCode;

        public AdministrationController(ServiceProvider serviceProvider, AdminAccessCode adminCode)
        {
            _serviceProvider = serviceProvider;
            _adminCode = adminCode;
        }

        [HttpGet("create")]
        public IActionResult GetOrCreateUser([FromQuery]string email,[FromQuery]string admKey)
        {
            if (_adminCode.AdminCode != admKey)
            {
                return Unauthorized("Your admin code is not correct");
            }
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
                var licenceId = _serviceProvider.GetLicenceService().GetFirstUnusedLicenceKey()?.Id;
                if (!licenceId.HasValue)
                {
                    licenceId = _serviceProvider.GetLicenceService().GenerateLicence();
                }
                _serviceProvider.GetUserService().AssignLicence(user.Id, licenceId.Value);
                _serviceProvider.GetLicenceService().SetUsed(licenceId.Value);
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

        [HttpGet]
        public IActionResult GetUser([FromQuery]string email, [FromQuery]string admKey)
        {
            if (_adminCode.AdminCode != admKey)
            {
                return Unauthorized("Your admin code is not correct");
            }
            _serviceProvider.GetUserService().GetOrCreateUser(email);
            var user = _serviceProvider.GetUserService().GetUser(email);
            var result = new UserModel
            {
                Id = user.Id,
                Mail = user.Mail,
                Licence = new LicenceModel()
            };
            LicenceKey licence;
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

        [HttpPost("resignlicence")]
        public IActionResult ResignLicence([FromQuery]string email, [FromQuery]string admKey)
        {
            if (_adminCode.AdminCode != admKey)
            {
                return Unauthorized("Your admin code is not correct");
            }
            var user = _serviceProvider.GetUserService().GetUser(email);
            if (user.LicenceKeyId.HasValue)
            {
                _serviceProvider.GetLicenceService().SetUnused(user.LicenceKeyId.Value);
            }
            _serviceProvider.GetUserService().ResignLicence(user.Id);
            return Ok();
        }

        [HttpPost("assignlicence")]
        public IActionResult AssignLicence([FromQuery]string email, [FromQuery]string admKey)
        {
            if (_adminCode.AdminCode != admKey)
            {
                return Unauthorized("Your admin code is not correct");
            }
            var user = _serviceProvider.GetUserService().GetUser(email);
            var licence = _serviceProvider.GetLicenceService().GetFirstUnusedLicenceKey();
            if (licence != null)
            {
                _serviceProvider.GetUserService().AssignLicence(user.Id, licence.Id);
                _serviceProvider.GetLicenceService().SetUsed(licence.Id);
                return Ok();
            }
            else
            {
                return BadRequest("No unassigned licence was found.");
            }
        }
    }
}