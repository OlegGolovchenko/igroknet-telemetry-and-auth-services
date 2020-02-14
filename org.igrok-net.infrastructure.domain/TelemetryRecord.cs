namespace org.igrok_net.infrastructure.domain
{
    public class TelemetryRecord
    {
        public long Id { get; private set; }
        public string OsVersion { get; private set; }
        public string NetFxVersion { get; private set; }
        public long UserId { get; private set; }

        public TelemetryRecord(long userId)
        {
            UserId = userId;
        }

        internal TelemetryRecord(long telRecId, string osVersion, string netFxVersion, long userId)
        {
            Id = telRecId;
            OsVersion = osVersion;
            NetFxVersion = netFxVersion;
            UserId = userId;
        }

        public void SetOsVersion(string osVersion)
        {
            OsVersion = osVersion;
        }

        public void SetNetFxVersion(string netFxVersion)
        {
            NetFxVersion = netFxVersion;
        }
    }
}
