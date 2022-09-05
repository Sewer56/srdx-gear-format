using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using SrdxGearPacker;

var parser = new Parser(with =>
{
    with.AutoHelp = true;
    with.CaseSensitive = false;
    with.CaseInsensitiveEnumValues = true;
    with.EnableDashDash = true;
    with.HelpWriter = null;
});

var parserResult = parser.ParseArguments<ExtractOptions, PackOptions>(args);
parserResult.WithParsed<ExtractOptions>(Packer.Extract)
    .WithParsed<PackOptions>(Packer.Pack)
    .WithNotParsed(errs => HandleParseError(parserResult, errs));

static void HandleParseError(ParserResult<object> options, IEnumerable<Error> errs)
{
    var helpText = HelpText.AutoBuild(options, help =>
    {
        help.Copyright = "Created by Sewer56, licensed under MIT License";
        help.AutoHelp = false;
        help.AutoVersion = false;
        help.AddDashesToOption = true;
        help.AddEnumValuesToHelpText = true;
        help.AdditionalNewLineAfterOption = true;
        return HelpText.DefaultParsingErrorsHandler(options, help);
    }, example => example, true);

    Console.WriteLine(helpText);
}