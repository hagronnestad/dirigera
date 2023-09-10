namespace Dirigera.Lib.Devices.Base
{
    public class Attributes
    {
        public string? CustomName { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? HardwareVersion { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? ProductCode { get; set; }
        public string? SerialNumber { get; set; }

        public string? OtaPolicy { get; set; }
        public int OtaProgress { get; set; }
        public string? OtaScheduleEnd { get; set; }
        public string? OtaScheduleStart { get; set; }
        public string? OtaState { get; set; }
        public string? OtaStatus { get; set; }
    }
}
