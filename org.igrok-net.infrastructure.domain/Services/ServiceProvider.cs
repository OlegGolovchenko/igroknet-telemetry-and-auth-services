using org.igrok_net.infrastructure.domain.Interfaces;

namespace org.igrok_net.infrastructure.domain.Services
{
    public class ServiceProvider
    {
        private ILicenceService _licenceService;
        private ITelemetryService _telemetryService;
        private IUserService _userService;
        private IDataAccess _dataConnection;

        public ServiceProvider(IDataAccess dataConnection)
        {
            _dataConnection = dataConnection;
            _licenceService = new LicenceService(_dataConnection);
            _telemetryService = new TelemetryService(_dataConnection);
            _userService = new UserService(_dataConnection);
        }

        public ILicenceService GetLicenceService()
        {
            return _licenceService;
        }

        public ITelemetryService GetTelemetryService()
        {
            return _telemetryService;
        }

        public IUserService GetUserService()
        {
            return _userService;
        }
    }
}
