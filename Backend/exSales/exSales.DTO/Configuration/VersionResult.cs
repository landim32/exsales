using System;
using System.Text.Json.Serialization;
using exSales.DTO.Domain;

namespace exSales.DTO.Configuration
{
    public class VersionResult : StatusResult
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
