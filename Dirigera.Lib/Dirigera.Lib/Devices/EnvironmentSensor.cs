using Dirigera.Lib;
using Dirigera.Lib.Api.Dto.Base;
using Dirigera.Lib.Devices.Base;

namespace Dirigera.Models
{
    public class EnvironmentSensor : Device
    {
        public int CurrentTemperature { get; set; }
        public int CurrentRH { get; set; }
        public int CurrentPM25 { get; set; }
        public int MaxMeasuredPM25 { get; set; }
        public int MinMeasuredPM25 { get; set; }
        public int VocIndex { get; set; }

        internal EnvironmentSensor(DirigeraManager manager, DeviceDto dto) : base(manager, dto)
        {

        }

        internal override void PopulateFromDto(DeviceDto dto)
        {
            base.PopulateFromDto(dto);

            // Extract the "standard" attributes from the Attributes dictionary into their own properties
            if (dto.Attributes is not null)
            {
                if (dto.Attributes.ContainsKey("currentTemperature")) CurrentTemperature = dto.Attributes["currentTemperature"].GetInt32();
                if (dto.Attributes.ContainsKey("currentRH")) CurrentRH = dto.Attributes["currentRH"].GetInt32();
                if (dto.Attributes.ContainsKey("currentPM25")) CurrentPM25 = dto.Attributes["currentPM25"].GetInt32();
                if (dto.Attributes.ContainsKey("maxMeasuredPM25")) MaxMeasuredPM25 = dto.Attributes["maxMeasuredPM25"].GetInt32();
                if (dto.Attributes.ContainsKey("minMeasuredPM25")) MinMeasuredPM25 = dto.Attributes["minMeasuredPM25"].GetInt32();
                if (dto.Attributes.ContainsKey("vocIndex")) VocIndex = dto.Attributes["vocIndex"].GetInt32();
            }
        }
    }
}
