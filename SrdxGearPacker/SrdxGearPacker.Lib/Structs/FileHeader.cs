using System;
using System.Buffers.Binary;

namespace SrdxGearPacker.Lib.Structs;

/// <summary>
/// Describes the header of the custom gear file.
/// </summary>
public struct FileHeader
{
    public const int ExpectedMagic = 0x4D545343; // CSTM

    /// <summary>
    /// Constant value.
    /// </summary>
    public int Magic;

    /// <summary>
    /// Address of textures inside this file.
    /// </summary>
    public int TexturePtr;

    /// <summary>
    /// Checks if the magic number is correct.
    /// </summary>
    public bool IsCorrectMagic()
    {
        if (!BitConverter.IsLittleEndian)
            return Magic == BinaryPrimitives.ReverseEndianness(ExpectedMagic);

        return Magic == ExpectedMagic;
    }

    /// <summary>
    /// Swaps the endian.
    /// </summary>
    public void SwapEndianIfNeeded()
    {
        if (BitConverter.IsLittleEndian)
            TexturePtr = BinaryPrimitives.ReverseEndianness(TexturePtr);
    }
}