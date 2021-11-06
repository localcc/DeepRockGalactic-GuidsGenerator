using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UAssetAPI;
using UAssetAPI.PropertyTypes;

namespace DeepRockGalactic_GuidsGenerator
{
    internal static class AssetApiExtensions
    {
        public static PropertyData? GetPropertyData(this NormalExport export, string name)
        {
            return export.Data.Where(d => d.Name.Value.Value == name).FirstOrDefault();
        }

        public static Export? GetExportByName(this UAsset asset, string name)
        {
            return asset.Exports.Where(e => e.ObjectName.Value.Value == name).FirstOrDefault();
        }

        public static Export? GetExportByName(this UAsset asset, Func<string, bool> predicate)
        {
            return asset.Exports.Where(e => predicate(e.ObjectName.Value.Value)).FirstOrDefault();
        }

        private static Regex AssetRegex = new Regex("(?<=\\.)[^.]*$", RegexOptions.Compiled);
        private const string AssetRoot = "/Game/";
        public static string AssetPathToLocal(this string a)
        {
            return AssetRegex.Replace(a.Substring(AssetRoot.Length, a.Length - AssetRoot.Length).Replace("/", "\\"), "uasset");
        }

        public static string? ResolveText(this TextPropertyData data, string rootDir)
        {
            switch (data.HistoryType)
            {
                case TextHistoryType.Base:
                    return data.CultureInvariantString.Value;
                case TextHistoryType.StringTableEntry:
                    var localizationKey = data.Value.Value;
                    var assetPath = data.TableId.Value.Value;
                    var localPath = Path.Combine(rootDir, assetPath.AssetPathToLocal());
                    var localizationTable = new LocalizationTableAsset(localPath);
                    return localizationTable.GetEntry(localizationKey);
                default:
                    throw new ArgumentException("Can't resolve this text type!");
            }
        }
    }
}
