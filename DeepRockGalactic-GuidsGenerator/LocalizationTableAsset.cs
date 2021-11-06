using System;
using System.Linq;
using UAssetAPI;

namespace DeepRockGalactic_GuidsGenerator
{
    internal class LocalizationTableAsset
    {
        private UAsset _asset;
        private StringTableExport _tableExport;
        public LocalizationTableAsset(string gearDataPath)
        {
            _asset = new UAsset(gearDataPath);
            _tableExport = _asset.Exports.FirstOrDefault() as StringTableExport ?? throw new ArgumentException("Invalid GearData!");
        }

        public string? GetEntry(string key)
        {
            var index = _tableExport.Data2.FindIndex(s => s.Value == key);
            if (index == -1 || index == _tableExport.Data2.Count - 1) return null;
            return _tableExport.Data2[index + 1].Value;
        }
    }
}
