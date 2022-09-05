using System;
using System.IO;
using SrdxGearPacker.Lib.Structs;

namespace SrdxGearPacker.Lib;

public unsafe class GearFileReader
{
    /// <summary>
    /// Number of textures inside this file.
    /// </summary>
    public int TextureCount { get; set; }

    /// <summary>
    /// The model contained inside this gear.
    /// </summary>
    private byte* Model { get; set; }

    private byte*[] Textures { get; set; }

    private FileHeader* _fileHeader;
    private TextureHeader* _textureHeader;

    /// <summary>
    /// Parses a gear file provided in the byte array.
    /// </summary>
    /// <param name="file">Byte array containing the file.</param>
    public unsafe GearFileReader(byte* file)
    {
        _fileHeader = (FileHeader*)file;
        var header = *_fileHeader;
        if (!header.IsCorrectMagic())
            throw new Exception("Invalid File");

        Model = (file + Constants.GameCubeAlignment);
        header.SwapEndianIfNeeded();

        _textureHeader = (TextureHeader*)(file + header.TexturePtr);
        var textureHeader = *_textureHeader;
        textureHeader.SwapEndianIfNeeded();
        Textures = new byte*[textureHeader.Count];

        TextureCount = textureHeader.Count;
        Span<int> offsets = stackalloc int[textureHeader.Count];
        textureHeader.GetTexturePtrs(_textureHeader, offsets);

        for (int x = 0; x < offsets.Length; x++)
            Textures[x] = (file + offsets[x]);
    }

    /// <summary>
    /// Gets a span to a given texture.
    /// </summary>
    public Span<byte> GetModel()
    {
        // Feeling lazy to write code to calculate size of NN files.
        return new Span<byte>(Model, (int)((byte*)_textureHeader - Model));
    }

    /// <summary>
    /// Gets a span to a given texture.
    /// </summary>
    public Span<byte> GetTexture(int index)
    {
        var ptr = Textures[index];
        return new Span<byte>(ptr, Utilities.GetTextureSize(ptr));
    }

    /// <summary>
    /// Extracts the gear archive to a given directory.
    /// </summary>
    /// <param name="saveFolder">Folder to place gear archive data in.</param>
    public void ExtractToDirectory(string saveFolder)
    {
        Directory.CreateDirectory(saveFolder);
        Utilities.WriteAllBytes(Constants.GetModelPath(saveFolder), GetModel());
        for (int x = 0; x < TextureCount; x++)
            Utilities.WriteAllBytes(Constants.GetTexturePath(saveFolder, x), GetTexture(x));
    }
}