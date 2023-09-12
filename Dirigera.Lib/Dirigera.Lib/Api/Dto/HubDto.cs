using Dirigera.Lib.Api.Dto.Base;

namespace Dirigera.Lib.Api.Dto
{
    internal class HubDto : DeviceDto
    {
        public string? ApiVersion { get; set; }
    }
}
