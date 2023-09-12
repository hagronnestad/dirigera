using Dirigera.Lib;
using Dirigera.Lib.Api.Dto.Base;
using Dirigera.Lib.Devices.Base;

namespace Dirigera.Models
{
    public class Light : Device
    {
        public bool IsOn { get; internal set; }
        public int LightLevel { get; internal set; }
        public string? ColorMode { get; internal set; }
        public double ColorHue { get; internal set; }
        public double ColorSaturation { get; internal set; }
        public int ColorTemperature { get; internal set; }
        public int ColorTemperatureMax { get; internal set; }
        public int ColorTemperatureMin { get; internal set; }


        internal Light(DirigeraManager manager, DeviceDto dto) : base(manager, dto)
        {

        }

        public async Task TurnOff()
        {
            await _manager.SetLightState(this, false);
        }

        public async Task TurnOn()
        {
            await _manager.SetLightState(this, true);
        }

        public async Task Toggle()
        {
            await Refresh();
            await _manager.SetLightState(this, !IsOn);
            await Refresh();
        }

        internal override void PopulateFromDto(DeviceDto dto)
        {
            base.PopulateFromDto(dto);

            // Extract the "standard" attributes from the Attributes dictionary into their own properties
            if (dto.Attributes is not null)
            {
                if (dto.Attributes.ContainsKey("isOn")) IsOn = dto.Attributes["isOn"].GetBoolean();
                if (dto.Attributes.ContainsKey("lightLevel")) LightLevel = dto.Attributes["lightLevel"].GetInt32();
                if (dto.Attributes.ContainsKey("colorMode")) ColorMode = dto.Attributes["colorMode"].GetString();
                if (dto.Attributes.ContainsKey("colorHue")) ColorHue = dto.Attributes["colorHue"].GetDouble();
                if (dto.Attributes.ContainsKey("colorSaturation")) ColorSaturation = dto.Attributes["colorSaturation"].GetDouble();
                if (dto.Attributes.ContainsKey("colorTemperature")) ColorTemperature = dto.Attributes["colorTemperature"].GetInt32();
                if (dto.Attributes.ContainsKey("colorTemperatureMax")) ColorTemperatureMax = dto.Attributes["colorTemperatureMax"].GetInt32();
                if (dto.Attributes.ContainsKey("colorTemperatureMin")) ColorTemperatureMin = dto.Attributes["colorTemperatureMin"].GetInt32();
            }
        }
    }
}
