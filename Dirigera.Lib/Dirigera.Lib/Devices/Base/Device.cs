using Dirigera.Lib.Api.Dto.Base;
using System.Text.Json;

namespace Dirigera.Lib.Devices.Base
{
    public class Device
    {
        internal readonly DirigeraManager _manager;

        public string Id { get; set; }
        public string Type { get; set; }
        public string DeviceType { get; set; }
        public bool IsReachable { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset LastSeen { get; set; }
        public Dictionary<string, JsonElement>? Attributes { get; set; }


        public string? Name { get; set; }
        public string? Room { get; set; }


        internal Device(DirigeraManager manager, DeviceDto dto)
        {
            _manager = manager;

            Id = dto.Id;
            Type = dto.Type;
            DeviceType = dto.DeviceType;
            IsReachable = dto.IsReachable;
            CreatedAt = dto.CreatedAt;
            LastSeen = dto.LastSeen;
            Attributes = dto.Attributes;

            if (dto.Attributes is not null)
            {
                if (dto.Attributes.ContainsKey("customName")) Name = dto.Attributes["customName"].GetString();
            }

            Room = dto.Room?.Name;
        }
    }
}
