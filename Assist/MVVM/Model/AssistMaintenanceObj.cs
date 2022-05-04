using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Assist.MVVM.Model
{
    public class AssistMaintenanceObj
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("downForMaintenance")]
        public bool DownForMaintenance { get; set; }

        [JsonPropertyName("downForMaintenanceMessage")]
        public string DownForMaintenanceMessage { get; set; }
    }
}
