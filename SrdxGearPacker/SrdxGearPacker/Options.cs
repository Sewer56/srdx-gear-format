using CommandLine;

namespace SrdxGearPacker;

[Verb("extract")]
internal class ExtractOptions
{
    [Option(Required = true)]
    public string Source { get; set; }

    [Option(Required = true)]
    public string SavePath { get; set; }
}

[Verb("pack")]
internal class PackOptions
{
    [Option(Required = true)]
    public string Source { get; set; }

    [Option(Required = true)]
    public string SavePath { get; set; }
}