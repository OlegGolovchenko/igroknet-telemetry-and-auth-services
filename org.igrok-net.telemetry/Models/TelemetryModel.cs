﻿using System.Collections.Generic;

namespace org.igrok_net.telemetry.Models
{
    public class TelemetryModel
    {
        public string Email { get; set; }
        public string OsVersion { get; set; }
        public string NetFxVersion { get; set; }

        public IEnumerable<TelemetryIpModel> TelemetryIps { get; set; }
    }
}
