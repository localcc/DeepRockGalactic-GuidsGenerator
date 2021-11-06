using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeepRockGalactic_GuidsGenerator
{
    internal class MatrixData
    {
        [JsonPropertyName("overclocks")]
        public Dictionary<string, OverclockData> Overclocks { get; set; }
        [JsonPropertyName("cosmetics")]
        public Dictionary<string, CosmeticData> Cosmetics { get; set; }
    }
}
