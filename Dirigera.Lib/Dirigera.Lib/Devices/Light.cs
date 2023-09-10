using Dirigera.Lib;
using Dirigera.Lib.Api.Dto.Base;
using Dirigera.Lib.Devices.Base;

namespace Dirigera.Models
{
    public class Light : Device
    {
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
            var state = _manager.Lights?.FirstOrDefault(x => x.Id == Id)?.Attributes?["isOn"].GetBoolean();
            if (state is null) return;
            await _manager.SetLightState(this, !state.Value);
            await _manager.Refresh();
        }
    }
}
