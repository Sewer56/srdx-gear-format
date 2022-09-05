using System.IO;

namespace SrdxGearPacker.Lib;

internal class Constants
{
    public const int GameCubeAlignment = 32;

    public const string TextureExtension = ".gvr";
    public const string ModelName = "Model.gno";
    public const string TextureFormat = "00000";

    public static string GetModelPath(string folder) => Path.Combine(folder, ModelName);

    public static string GetTexturePath(string folder, int index) => Path.Combine(folder, $"{index.ToString(TextureFormat)}.gvr");
}