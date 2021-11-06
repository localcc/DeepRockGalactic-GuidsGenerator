using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace DeepRockGalactic_GuidsGenerator
{
    internal class CosmeticGenerator
    {
        private string _rootDir;
        private string[] _sccPaths;
        public CosmeticGenerator(string rootDir)
        {
            this._rootDir = rootDir;
            this._sccPaths = Directory.GetFiles(Path.Combine(_rootDir, "WeaponsNTools", "_GlobalSkins"), "SCC_*.uasset");
        }

        private string? GetCosmeticName(UAsset asset, NormalExport schematicExport, string parentDir)
        {
            var itemProperty = schematicExport.GetPropertyData("Item") as ObjectPropertyData;
            var itemExport = itemProperty?.ToExport(asset) as NormalExport;
            var cosmeticFileNameProperty = itemExport?.GetPropertyData("Skin") as ObjectPropertyData;
            var cosmeticFileNameImport = cosmeticFileNameProperty?.ToImport(asset);
            var cosmeticFileName = cosmeticFileNameImport?.ObjectName.Value.Value;
            if (cosmeticFileName == null) return null;

            var cosmeticAsset = new UAsset(Path.Combine(parentDir, cosmeticFileName + ".uasset"));
            var skinExport = cosmeticAsset.GetExportByName(cosmeticFileName) as NormalExport;
            var skinName = skinExport?.GetPropertyData("SkinName") as TextPropertyData;

            return skinName?.ResolveText(this._rootDir);
        }
       

        private CosmeticData? GetCosmeticData(UAsset asset, NormalExport schematicExport, string parentDir)
        {
            var name = GetCosmeticName(asset, schematicExport, parentDir);
            if(name == null)
            {
                Console.Error.WriteLine($"Failed to get cosmetic name for: {parentDir}");
                name = $"FIXME{parentDir}";
            }

            var playerClass = schematicExport.GetPlayerClass(asset);
            if(playerClass == null)
            {
                Console.Error.WriteLine($"Failed to get cosmetic player class for: {parentDir}");
                playerClass = $"FIXME{parentDir}";
            }

            var cost = MatrixCoreCost.FromExport(asset, schematicExport);
            if(cost == null)
            {
                Console.Error.WriteLine($"Failed to get cost for: {parentDir}");
                return null;
            }

            var guidString = schematicExport.GetSaveGameID()?.ToStringCustom();
            if (guidString == null) return null;

            return new CosmeticData
            {
                Name = name,
                Class = playerClass,
                Cost = cost,
                Guid = guidString
            };
        }

        public Dictionary<string, CosmeticData> GenerateCosmetics()
        {
            var dict = new Dictionary<string, CosmeticData>();

            foreach (var scc in this._sccPaths)
            {
                var parentDir = new DirectoryInfo(scc).Parent?.FullName;
                if(parentDir == null)
                {
                    Console.Error.WriteLine($"Failed to get parent dir for: {scc}");
                    continue;
                }
                var cosmeticAsset = new UAsset(scc);
                foreach(var schematicEntry in cosmeticAsset.GetExportsByName("Schematic"))
                {
                    var schematicExport = schematicEntry as NormalExport;
                    if (schematicExport == null)
                    {
                        Console.Error.WriteLine($"Failed to get schematic export for: {scc}");
                        continue;
                    }
                    var data = GetCosmeticData(cosmeticAsset, schematicExport, parentDir);
                    if(data == null)
                    {
                        Console.Error.WriteLine($"Failed to get cosmetic data for: {scc}");
                        continue;
                    }
                    dict.Add(data.Guid, data);
                }
            }
            return dict;
        }
    }
}
