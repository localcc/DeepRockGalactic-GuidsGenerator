using System.Text.Json.Serialization;

namespace DeepRockGalactic_GuidsGenerator
{
    internal class OverclockData : MatrixCore
    {
        [JsonPropertyName("weapon")]
        public string Weapon { get; set; }
    }
}
