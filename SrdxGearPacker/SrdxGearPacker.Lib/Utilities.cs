using System;
using System.Buffers.Binary;
using System.IO;

namespace SrdxGearPacker.Lib;

internal static class Utilities
{
    internal static int RoundUp(int number, int multiple)
    {
        if (multiple == 0)
            return number;

        int remainder = number % multiple;
        if (remainder == 0)
            return number;

        return number + multiple - remainder;
    }

    internal static unsafe int GetTextureSize(byte* ptr)
    {
        // Texture header specifies remaining space taken by texture at offset 20
        var size = *(int*)(ptr + 20);
        if (!BitConverter.IsLittleEndian)
            size = BinaryPrimitives.ReverseEndianness(size);

        return size + 24;
    }

    internal static void WriteAllBytes(string filePath, Span<byte> bytes)
    {
        using var fileStream = new FileStream(filePath, FileMode.Create);
        fileStream.Write(bytes);
    }

    internal static int AsLittleEndian(this int value)
    {
        if (!BitConverter.IsLittleEndian)
            return BinaryPrimitives.ReverseEndianness(value);

        return value;
    }

    internal static int AsBigEndian(this int value)
    {
        if (BitConverter.IsLittleEndian)
            return BinaryPrimitives.ReverseEndianness(value);

        return value;
    }
}