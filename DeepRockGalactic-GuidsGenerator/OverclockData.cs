using System.Text.Json.Serialization;

namespace DeepRockGalactic_GuidsGenerator
{
    public class OverclockData
    {
        [JsonPropertyName("class")]
        public string Class { get; set; }

        [JsonPropertyName("weapon")]
        public string Weapon { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cost")]
        public OverclockCost Cost { get; set; }

        [JsonIgnore]
        public string Guid { get; set; }
    }
}
