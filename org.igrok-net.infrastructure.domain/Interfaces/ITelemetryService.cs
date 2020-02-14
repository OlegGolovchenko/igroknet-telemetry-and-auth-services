namespace org.igrok_net.infrastructure.domain.Interfaces
{
    public interface ITelemetryService
    {
        long CreateOrUpdateTelemetryRecord(long userId, string osName, string netFxVersion);
        TelemetryRecord GetTelemetryRecord(long recordId);
        TelemetryRecord GetTelemetryRecordFor(long userId);
        bool RecordExists(long userId);
        void RemoveTelemetryRecord(long recordId);
    }
}
