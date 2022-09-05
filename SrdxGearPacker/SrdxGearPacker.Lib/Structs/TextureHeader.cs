using System;
using System.Buffers.Binary;

namespace SrdxGearPacker.Lib.Structs;

/// <summary>
/// Describes the header containing texture data.
/// </summary>
public struct TextureHeader
{
    /// <summary>
    /// Number of stored textures.
    /// </summary>
    public int Count;

    /// <summary>
    /// Gets the texture pointers from this header.
    /// </summary>
    /// <param name="addr">Address of this texture header.</param>
    /// <param name="buffer">Buffer to insert the texture pointers into.</param>
    public unsafe Span<int> GetTexturePtrs(TextureHeader* addr, Span<int> buffer)
    {
        var firstPtrAddr = (int*)(addr + 1);
        for (int x = 0; x < Count; x++)
        {
            if (BitConverter.IsLittleEndian)
                buffer[x] = BinaryPrimitives.ReverseEndianness(*firstPtrAddr);
            else
                buffer[x] = *firstPtrAddr;

            firstPtrAddr += 1;
        }

        return buffer.Slice(0, Count);
    }

    /// <summary>
    /// Swaps the endian.
    /// </summary>
    public void SwapEndianIfNeeded()
    {
        if (BitConverter.IsLittleEndian)
            Count = BinaryPrimitives.ReverseEndianness(Count);
    }
}