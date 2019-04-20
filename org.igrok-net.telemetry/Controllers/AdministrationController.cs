using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AdministrationController(ServiceProvider serviceProvider)
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
                _serviceProvider.GetUserService().AssignLicence(user.Id, licenceId);
                user = _serviceProvider.GetUserService().GetUser(email);
            }
            licence = _serviceProvider.GetLicenceService().GetLicenceKey(user.LicenceKeyId.Value);
            result.Licence = new LicenceModel
            {
                Id = licence.Id,
                IsUsed = licence.IsUsed,
                Key = licence.Key
            };
            return Ok(result);
        }
    }
}