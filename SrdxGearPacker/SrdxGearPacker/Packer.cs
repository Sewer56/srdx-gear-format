using System;
using System.IO;
using SrdxGearPacker.Lib;

namespace SrdxGearPacker;

internal static class Packer
{
    internal static unsafe void Extract(ExtractOptions opt)
    {
        var bytes = File.ReadAllBytes(opt.Source);
        fixed (byte* bytePtr = bytes)
        {
            var file = new GearFileReader(bytePtr);
            file.ExtractToDirectory(opt.SavePath);
        }
        Console.WriteLine("Done");
    }

    internal static void Pack(PackOptions opt)
    {
        GearFileCreator.Create(opt.Source, opt.SavePath);
        Console.WriteLine("Done");
    }
}