using System.Text.Json;

namespace Dirigera.Lib.Api.Dto.Base
{
    internal class DeviceDto
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string DeviceType { get; set; } = "";
        public string CustomIcon { get; set; } = "";
        public bool IsReachable { get; set; }
        public bool IsHidden { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset LastSeen { get; set; }

        public Dictionary<string, JsonElement>? Attributes { get; set; }
        public RoomDto? Room { get; set; }
    }
}
