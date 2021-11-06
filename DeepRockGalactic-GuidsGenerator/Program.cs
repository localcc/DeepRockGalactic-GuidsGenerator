using System;
using System.IO;
using System.Text.Json;

namespace DeepRockGalactic_GuidsGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
            if(args.Length != 1)
            {
                Console.WriteLine("Invalid argument amount!");
                Console.WriteLine("Usage: GuidGenerator.exe [unpacked_game_path]");
                Console.WriteLine("Example: GuidGenerator.exe unpacked/FSD/Content");
                return;
            }*/
            //string rootPath = args[0];
            string rootPath = "C:\\Users\\catherine\\Downloads\\Useful-Scripts-main\\DRGPackerV2\\unpacked\\FSD\\Content";
            
            var overclockGenerator = new OverclockGenerator(rootPath);
            var overclocksData = overclockGenerator.GenerateOverclocks();

            var cosmeticGenerator = new CosmeticGenerator(rootPath);
            var cosmeticData = cosmeticGenerator.GenerateCosmetics();

            var matrixData = new MatrixData { Cosmetics = cosmeticData, Overclocks = overclocksData };
            var matrixCores = JsonSerializer.Serialize(matrixData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("matrix_cores.json", matrixCores);
        }
    }
}
