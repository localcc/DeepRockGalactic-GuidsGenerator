using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DeepRockGalactic_GuidsGenerator
{
    public class OverclockCost
    {
        private static Dictionary<string, Action<OverclockCost, int>> CostSetters = new()
        {
            { "Credits", (cost, am) => cost.Credits = am },
            { "Jadiz", (cost, am) => cost.Jadiz = am },
            { "Magnite", (cost, am) => cost.Magnite = am },
            { "Enor", (cost, am) => cost.Enor = am },
            { "Bismor", (cost, am) => cost.Bismor = am },
            { "Umanite", (cost, am) => cost.Umanite = am },
            { "Croppa", (cost, am) => cost.Croppa = am }
        };

        [JsonPropertyName("credits")]
        public int Credits { get; set; }

        [JsonPropertyName("bismor")]
        public int Bismor { get; set; }

        [JsonPropertyName("croppa")]
        public int Croppa { get; set; }

        [JsonPropertyName("enor")]
        public int Enor { get; set; }

        [JsonPropertyName("jadiz")]
        public int Jadiz { get; set; }

        [JsonPropertyName("magnite")]
        public int Magnite { get; set; }

        [JsonPropertyName("umanite")]
        public int Umanite { get; set; }

        public bool SetCost(string currency, int amount)
        {
            if (!CostSetters.TryGetValue(currency, out var costSetter)) return false;
            costSetter(this, amount);
            return true;
        }
    }
}
