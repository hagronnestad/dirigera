using Dirigera.Lib;
using Dirigera.Lib.Api.Dto.Base;
using Dirigera.Lib.Devices.Base;

namespace Dirigera.Models
{
    public class Outlet : Device
    {
        public bool IsOn { get; internal set; }

        internal Outlet(DirigeraManager manager, DeviceDto dto) : base(manager, dto)
        {

        }

        public async Task TurnOff()
        {
            await _manager.SetOutlet(this, false);
        }

        public async Task TurnOn()
        {
            await _manager.SetOutlet(this, true);
        }

        public async Task Toggle()
        {
            await Refresh();
            await _manager.SetOutlet(this, !IsOn);
            await Refresh();
        }

        internal override void PopulateFromDto(DeviceDto dto)
        {
            base.PopulateFromDto(dto);

            // Extract the "standard" attributes from the Attributes dictionary into their own properties
            if (dto.Attributes is not null)
            {
                if (dto.Attributes.ContainsKey("isOn")) IsOn = dto.Attributes["isOn"].GetBoolean();
            }
        }
    }
}
