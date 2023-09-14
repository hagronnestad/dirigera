using Dirigera.Lib.Api.Dto.Base;
using System.Text.Json;

namespace Dirigera.Lib.Models.Base
{
    public class Device
    {
        internal readonly DirigeraManager _manager;

        public string? Id { get; internal set; }
        public string? Type { get; internal set; }
        public string? DeviceType { get; internal set; }
        public bool IsReachable { get; internal set; }
        public DateTimeOffset CreatedAt { get; internal set; }
        public DateTimeOffset LastSeen { get; internal set; }
        public Dictionary<string, JsonElement>? Attributes { get; internal set; }

        public string? Name { get; internal set; }
        public string? Model { get; internal set; }
        public string? Manufacturer { get; internal set; }
        public string? FirmwareVersion { get; internal set; }
        public string? HardwareVersion { get; internal set; }
        public string? SerialNumber { get; internal set; }
        public string? ProductCode { get; internal set; }
        public int? BatteryPercentage { get; internal set; }
        public string? OtaStatus { get; internal set; }
        public string? OtaState { get; internal set; }
        public int? OtaProgress { get; internal set; }
        public string? OtaPolicy { get; internal set; }
        public string? OtaScheduleStart { get; internal set; }
        public string? OtaScheduleEnd { get; internal set; }

        public string? RoomName { get; internal set; }
        public string? RoomId { get; internal set; }


        internal Device(DirigeraManager manager, DeviceDto dto)
        {
            _manager = manager;
            PopulateFromDto(dto);
        }

        public async Task Refresh()
        {
            if (Id is null) return;
            var dto = await _manager.ApiClient.GetDevice<DeviceDto>(Id);
            if (dto is not null) PopulateFromDto(dto);
        }

        internal virtual void PopulateFromDto(DeviceDto dto)
        {
            Id = dto.Id;
            Type = dto.Type;
            DeviceType = dto.DeviceType;
            IsReachable = dto.IsReachable;
            CreatedAt = dto.CreatedAt;
            LastSeen = dto.LastSeen;

            if (dto.Room is not null)
            {
                RoomId = dto.Room.Id;
                RoomName = dto.Room.Name;
            }

            Attributes = dto.Attributes;

            // Extract the "standard" attributes from the Attributes dictionary into their own properties
            if (dto.Attributes is not null)
            {
                if (dto.Attributes.ContainsKey("customName")) Name = dto.Attributes["customName"].GetString();
                if (dto.Attributes.ContainsKey("model")) Model = dto.Attributes["model"].GetString();
                if (dto.Attributes.ContainsKey("manufacturer")) Manufacturer = dto.Attributes["manufacturer"].GetString();
                if (dto.Attributes.ContainsKey("firmwareVersion")) FirmwareVersion = dto.Attributes["firmwareVersion"].GetString();
                if (dto.Attributes.ContainsKey("hardwareVersion")) HardwareVersion = dto.Attributes["hardwareVersion"].GetString();
                if (dto.Attributes.ContainsKey("serialNumber")) SerialNumber = dto.Attributes["serialNumber"].GetString();
                if (dto.Attributes.ContainsKey("productCode")) ProductCode = dto.Attributes["productCode"].GetString();
                if (dto.Attributes.ContainsKey("batteryPercentage")) BatteryPercentage = dto.Attributes["batteryPercentage"].GetInt32();
                if (dto.Attributes.ContainsKey("otaStatus")) OtaStatus = dto.Attributes["otaStatus"].GetString();
                if (dto.Attributes.ContainsKey("otaState")) OtaState = dto.Attributes["otaState"].GetString();
                if (dto.Attributes.ContainsKey("otaProgress")) OtaProgress = dto.Attributes["otaProgress"].GetInt32();
                if (dto.Attributes.ContainsKey("otaPolicy")) OtaPolicy = dto.Attributes["otaPolicy"].GetString();
                if (dto.Attributes.ContainsKey("otaScheduleStart")) OtaScheduleStart = dto.Attributes["otaScheduleStart"].GetString();
                if (dto.Attributes.ContainsKey("otaScheduleEnd")) OtaScheduleEnd = dto.Attributes["otaScheduleEnd"].GetString();
            }

            // Set Name to Model if a custom name is missing, this is the default behaviour of the official app
            if (string.IsNullOrWhiteSpace(Name)) Name = Model;
        }
    }
}
