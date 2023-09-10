using Dirigera.Lib.Devices.Base;

namespace Dirigera.Devices
{
    public class AttributesLight : Attributes
    {
        public int LightLevel { get; set; }
        public bool IsOn { get; set; }
        public string? StartupOnOff { get; set; }
        public string? ColorMode { get; set; }
        public double ColorHue { get; set; }
        public double ColorSaturation { get; set; }
        public int StartupTemperature { get; set; }
        public int ColorTemperature { get; set; }
        public int ColorTemperatureMax { get; set; }
        public int ColorTemperatureMin { get; set; }
    }
}
