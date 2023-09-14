﻿using Dirigera.Lib.Api.Dto;
using Dirigera.Lib.Models.Base;

namespace Dirigera.Lib.Models
{
    public class Hub : Device
    {
        public string? ApiVersion { get; internal set; }

        internal Hub(DirigeraManager dirigeraManager, HubDto dto) : base(dirigeraManager, dto)
        {
            ApiVersion = dto.ApiVersion;
        }
    }
}
