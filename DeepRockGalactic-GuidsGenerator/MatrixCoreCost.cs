using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace DeepRockGalactic_GuidsGenerator
{
    public class MatrixCoreCost
    {
        private static Dictionary<string, Action<MatrixCoreCost, int>> CostSetters = new()
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

        public static MatrixCoreCost? FromExport (UAsset asset, NormalExport export)
        {
            var craftingCostProperty = export.GetPropertyData("CraftingCost") as MapPropertyData;
            if (craftingCostProperty == null) return null;

            MatrixCoreCost cost = new MatrixCoreCost();
            foreach (var craftingCurrency in craftingCostProperty.Value)
            {
                var currencyProperty = craftingCurrency.Key as ObjectPropertyData;
                var currency = currencyProperty?.ToImport(asset).ObjectName.Value.Value.Split("_").Last();
                if (currency == null)
                {
                    Console.Error.WriteLine($"Failed to get currency for: {export}");
                    continue;
                }
                var currencyAmountProperty = craftingCurrency.Value as IntPropertyData;
                if (currencyAmountProperty == null)
                {
                    Console.Error.WriteLine($"Failed to get amount of curency for: {export}");
                    continue;
                }
                if (!cost.SetCost(currency, currencyAmountProperty.Value))
                {
                    Console.Error.WriteLine($"Failed to set cost for: {currency} {cost} {export}");
                }
            }
            return cost;
        }
    }
}
