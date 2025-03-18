using System;
using System.Collections.Generic;

namespace PMD
{
    public delegate void AllSensorsUpdatedEventHandler();
    public interface IPMD_Device
    {
        event AllSensorsUpdatedEventHandler AllSensorsUpdated;

        int Id { get; }
        Guid Guid { get; }
        string Name { get; }
        int FirmwareVersion { get; }
        int MonitoringInterval { get; set; }
        List<Sensor> Sensors { get; }

        bool StartMonitoring();
        bool StopMonitoring();
    }
}