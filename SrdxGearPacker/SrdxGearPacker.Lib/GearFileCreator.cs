using System;
using System.IO;
using System.Linq;
using SrdxGearPacker.Lib.Structs;

namespace SrdxGearPacker.Lib;

/// <summary>
/// Utility class for creating gear files.
/// </summary>
public static class GearFileCreator
{
    /// <summary>
    /// Creates a gear archive.
    /// </summary>
    /// <param name="inFolder">Folder to create archive from.</param>
    /// <param name="outPath">Where to output the final file.</param>
    public static unsafe void Create(string inFolder, string outPath)
    {
        var textures     = Directory.GetFiles(inFolder, $"*{Constants.TextureExtension}");
        var textureFiles = textures.OrderBy(s => int.Parse(Path.GetFileNameWithoutExtension(s))).Select(File.ReadAllBytes).ToArray();
        var modelFile    = File.ReadAllBytes(Constants.GetModelPath(inFolder));

        // Make File
        Span<int> textureOffsets = stackalloc int[textureFiles.Length];
        var fileSize = CalcPositions(textureFiles, modelFile, textureOffsets, out int textureHeaderOffset);
        var outArray = new byte[fileSize];

        fixed (byte* bytePtr = &outArray[0])
        {
            // Write Header
            *(int*)bytePtr = FileHeader.ExpectedMagic.AsLittleEndian();
            *(int*)(bytePtr + 4) = textureHeaderOffset.AsBigEndian();

            // Write Model File to Result
            modelFile.CopyTo(new Span<byte>(bytePtr + Constants.GameCubeAlignment, modelFile.Length));

            // Write Texture Header
            var textureHeaderPtr = bytePtr + textureHeaderOffset;
            *(int*)textureHeaderPtr = textureFiles.Length.AsBigEndian();

            // Write Texture Offsets
            var textureOffsetPtr = textureHeaderPtr + sizeof(int);
            for (int x = 0; x < textureFiles.Length; x++)
            {
                *(int*)textureOffsetPtr = textureOffsets[x].AsBigEndian();
                textureOffsetPtr += sizeof(int);
            }

            // Write Textures
            for (int x = 0; x < textureOffsets.Length; x++)
                textureFiles[x].CopyTo(new Span<byte>(bytePtr + textureOffsets[x], textureFiles[x].Length));
        }

        // Write File
        File.WriteAllBytes(outPath, outArray);
    }

    /// <summary>
    /// Calculates positions of items inside array.
    /// </summary>
    /// <param name="textureFiles">Texture files.</param>
    /// <param name="modelFile">Model file for this archive.</param>
    /// <param name="textureOffsets">Span of all texture offsets.</param>
    /// <param name="textureHeaderOffset">Pointer to texture header</param>
    /// <returns>Length of file.</returns>
    private static unsafe int CalcPositions(byte[][] textureFiles, Span<byte> modelFile, Span<int> textureOffsets, out int textureHeaderOffset)
    {
        textureHeaderOffset   = Constants.GameCubeAlignment + modelFile.Length;
        var textureHeaderSize = sizeof(TextureHeader) + (textureOffsets.Length * sizeof(int));

        var firstTexturePtr  = Utilities.RoundUp(textureHeaderSize + textureHeaderOffset, Constants.GameCubeAlignment);
        var currentTextureOffset = firstTexturePtr;
        for (int x = 0; x < textureFiles.Length; x++)
        {
            textureOffsets[x] = currentTextureOffset;
            currentTextureOffset = Utilities.RoundUp(currentTextureOffset + textureFiles[x].Length, Constants.GameCubeAlignment);
        }

        return currentTextureOffset;
    }
}