using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeepRockGalactic_GuidsGenerator
{
    internal abstract class MatrixCore
    {
        [JsonPropertyName("class")]
        public string Class { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cost")]
        public MatrixCoreCost Cost { get; set; }

        [JsonIgnore]
        public string Guid { get; set; }
    }
}
