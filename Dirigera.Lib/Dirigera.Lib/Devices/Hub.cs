using Dirigera.Lib.Api.Dto;
using Dirigera.Lib.Devices.Base;

namespace Dirigera.Lib.Devices
{
    public class Hub : Device
    {
        public string? ApiVersion { get; set; }

        public Hub(DirigeraManager dirigeraManager, HubDto dto) : base(dirigeraManager, dto)
        {
            ApiVersion = dto.ApiVersion;
        }
    }
}
