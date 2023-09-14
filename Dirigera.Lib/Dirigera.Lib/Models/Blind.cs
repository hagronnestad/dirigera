using Dirigera.Lib;
using Dirigera.Lib.Api.Dto.Base;
using Dirigera.Lib.Models.Base;

namespace Dirigera.Models
{
    public class Blind : Device
    {
        public int? BlindsTargetLevel { get; internal set; }
        public int? BlindsCurrentLevel { get; internal set; }
        public string? BlindsState { get; internal set; }


        internal Blind(DirigeraManager manager, DeviceDto dto) : base(manager, dto)
        {

        }

        internal override void PopulateFromDto(DeviceDto dto)
        {
            base.PopulateFromDto(dto);

            // Extract the "standard" attributes from the Attributes dictionary into their own properties
            if (dto.Attributes is not null)
            {
                if (dto.Attributes.ContainsKey("blindsTargetLevel")) BlindsTargetLevel = dto.Attributes["blindsTargetLevel"].GetInt32();
                if (dto.Attributes.ContainsKey("blindsCurrentLevel")) BlindsCurrentLevel = dto.Attributes["blindsCurrentLevel"].GetInt32();
                if (dto.Attributes.ContainsKey("blindsState")) BlindsState = dto.Attributes["blindsState"].GetString();
            }
        }

        public async Task Open()
        {
            await _manager.SetBlind(this, 0);
        }

        public async Task Close()
        {
            await _manager.SetBlind(this, 100);
        }

        public async Task Set(int level)
        {
            await _manager.SetBlind(this, level);
        }
    }
}
