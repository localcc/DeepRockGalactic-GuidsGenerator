using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace DeepRockGalactic_GuidsGenerator
{
    internal class OverclockGenerator
    {
        private string _rootDir;
        private string[] _weaponsNToolsDirs;
        public OverclockGenerator(string rootDir)
        {
            this._rootDir = rootDir;
            this._weaponsNToolsDirs = Directory.GetDirectories(Path.Combine(rootDir, "WeaponsNTools"))
                .Where(d => !new DirectoryInfo(d).Name.StartsWith("_"))
                .Where(d => Directory.Exists(Path.Combine(d, "Overclocks")))
                .ToArray();
        }

        private List<NormalExport>? GetOverclocks(UAsset descriptor)
        {
            var overclocksList = new List<NormalExport>();

            var header = descriptor.Exports[0] as NormalExport;
            if (header == null) return null;
            var indices = (header.Data[0] as MapPropertyData)?.Value?.Values;
            if (indices == null) return null;

            foreach (var index in indices)
            {
                var objData = (ObjectPropertyData)index;
                var overclock = objData.ToExport(descriptor) as NormalExport;
                if (overclock == null)
                {
                    Console.Error.WriteLine($"Warning: failed to get overclcok at index {(int)index.RawValue - 1}");
                    continue;
                }
                overclocksList.Add(overclock);
            }
            return overclocksList;
        }

        private string? GetWeaponName(string weaponDir)
        {
            var weaponName = new DirectoryInfo(weaponDir).Name;
            var weaponAssetPath = Directory.GetFiles(weaponDir, "WPN_*.uasset").FirstOrDefault();
            if (weaponAssetPath == null) return null;

            var weaponAsset = new UAsset(weaponAssetPath);
            if (weaponAsset == null) return null;

            var weaponRoot = weaponAsset.GetExportByName((s) => s.StartsWith("Default__WPN_") && s.EndsWith("_C")) as NormalExport;
            var weaponItemProperty = weaponRoot?.GetPropertyData("UpgradableItem") as ObjectPropertyData;
            var weaponItem = weaponItemProperty?.ToExport(weaponAsset) as NormalExport;
            var localizedWeaponNameProperty = weaponItem?.GetPropertyData("Name") as TextPropertyData;
            return localizedWeaponNameProperty?.ResolveText(this._rootDir);
        }

        private string? GetPlayerClass(UAsset asset, NormalExport export)
        {
            var usedByProperty = export.GetPropertyData("UsedByCharacter") as ObjectPropertyData;
            var usedBy = usedByProperty?.ToImport(asset).ObjectName.Value.Value;
            return usedBy?.Substring(0, usedBy.Length - 2);
        }

        private UAsset? GetOverclockAsset(UAsset rootAsset, NormalExport overclockExport, string weaponDir)
        {
            var overclockDir = Path.Combine(weaponDir, "Overclocks");

            var itemExportProperty = overclockExport.GetPropertyData("Item") as ObjectPropertyData;
            var itemExport = itemExportProperty?.ToExport(rootAsset) as NormalExport;
            if (itemExport == null) return null;

            var itemProperty = itemExport.GetPropertyData("Overclock") as ObjectPropertyData;
            var item = itemProperty?.ToImport(rootAsset).ObjectName.Value.Value;
            if (item == null) return null;

            var overclockAsset = new UAsset(Path.Combine(overclockDir, item + ".uasset"));
            return overclockAsset;
        }

        private string? GetOverclockName(UAsset overclockAsset)
        {
            var overclockExport = overclockAsset.GetExportByName((e) => e.StartsWith("OC_")) as NormalExport;
            var overclockNameProperty = overclockExport?.GetPropertyData("Name") as TextPropertyData;
            return overclockNameProperty?.ResolveText(this._rootDir);
        }

        private OverclockCost? OverclockCostFromExport(UAsset asset, NormalExport export)
        {
            var craftingCostProperty = export.GetPropertyData("CraftingCost") as MapPropertyData;
            if (craftingCostProperty == null) return null;

            OverclockCost cost = new OverclockCost();
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

        private OverclockData? OverclockFromExport(UAsset asset, NormalExport export, string weaponDir)
        {
            var propertyData = export.GetPropertyData("SaveGameID") as StructPropertyData;
            var guid = (propertyData?.Value.FirstOrDefault() as GuidPropertyData)?.Value;
            if (guid == null) return null;

            var guidString = guid.Value.ToStringCustom();

            var weaponName = GetWeaponName(weaponDir);
            if (weaponName == null)
            {
                Console.Error.WriteLine($"Failed to get weapon name for: {weaponDir}");
                weaponName = $"FIXME{weaponDir}";
            }

            var playerClass = GetPlayerClass(asset, export);
            if (playerClass == null)
            {
                Console.Error.WriteLine($"Failed to get player class for: {weaponDir}");
                playerClass = $"FIXME{weaponDir}";
            }

            var overclockAsset = GetOverclockAsset(asset, export, weaponDir);
            if (overclockAsset == null)
            {
                Console.Error.WriteLine($"Failed to get overclock for: {weaponDir}");
                return null;
            }

            var overclockName = GetOverclockName(overclockAsset);
            if (overclockName == null)
            {
                Console.Error.WriteLine($"Failed to get overclock name for: {weaponDir}");
                overclockName = $"FIXME{weaponDir}";
            }

            var cost = OverclockCostFromExport(asset, export);
            if (cost == null)
            {
                Console.Error.WriteLine($"Failed to get overclock cost for {weaponDir}");
                return null;
            }

            return new OverclockData()
            {
                Cost = cost,
                Class = playerClass,
                Weapon = weaponName,
                Name = overclockName,
                Guid = guidString
            };
        }

        public Dictionary<string, OverclockData> GenerateOverclocks()
        {
            var dict = new Dictionary<string, OverclockData>();

            foreach (var equipmentDir in _weaponsNToolsDirs)
            {
                var dirName = new DirectoryInfo(equipmentDir).Name;
                var overclockDir = Path.Combine(equipmentDir, "Overclocks");

                foreach (var overclockDescriptor in Directory.GetFiles(overclockDir, "OSB_*.uasset"))
                {
                    var asset = new UAsset(overclockDescriptor);
                    var overclocks = GetOverclocks(asset);
                    if (overclocks == null)
                    {
                        Console.Error.WriteLine($"Failed to get overclocks for: {overclockDescriptor}");
                        continue;
                    }

                    foreach (var overclock in overclocks)
                    {
                        var overclockData = OverclockFromExport(asset, overclock, equipmentDir);
                        if (overclockData == null) continue;
                        dict.Add(overclockData.Guid, overclockData);
                    }
                }
            }
            return dict;
        }
    }
}
