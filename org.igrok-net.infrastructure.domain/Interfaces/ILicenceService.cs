namespace org.igrok_net.infrastructure.domain.Interfaces
{
    public interface ILicenceService
    {
        long GenerateLicence();
        void SetUsed(long licenceId);
        void SetUnused(long licenceId);
        LicenceKey GetLicenceKey(long licenceId);
        LicenceKey GetFirstUnusedLicenceKey();
    }
}
