using System;
using System.IO;
using System.Text.Json;

namespace DeepRockGalactic_GuidsGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                Console.WriteLine("Invalid argument amount!");
                Console.WriteLine("Usage: GuidGenerator.exe [unpacked_game_path]");
                Console.WriteLine("Example: GuidGenerator.exe unpacked/FSD/Content");
                return;
            }
            string rootPath = args[0];
            var generator = new OverclockGenerator(rootPath);
            var overclocks = generator.GenerateOverclocks();
            var overclocksJson = JsonSerializer.Serialize(overclocks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("guids.json", overclocksJson);
        }
    }
}
