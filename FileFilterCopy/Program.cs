using FileFilterCopy.Commands;
using Microsoft.Extensions.Logging;

const string SourceDirectionPath = @"D:\Programming projects";
const string DestinationDirectionPath = @"F:\Programming projects";
const bool SkipExistingDirectories = false;
const bool SkipExistingFiles = false;

string[] ignorePatterns =
[
    // Paths
    "D:/Programming projects/.git/",
    "D:/Programming projects/.gitignore",
    "D:/Programming projects/C#/Ip Country Dataset/Ip Country Dataset/Resources/ipv4_ranges_by_country/",
    "D:/Programming projects/C#/Ip Country Dataset/Ip Country Dataset/Resources/ipv6_ranges_by_country/",
    "D:/Programming projects/C#/Ip Country Dataset/Ip Country Dataset/Resources/aggregate_ipv4.csv",
    "D:/Programming projects/C#/Ip Country Dataset/Ip Country Dataset/Resources/aggregate_ipv6.csv",

    // Directories
    "/[Bb]in/",
    "/[Oo]bj/",
    "/node_modules/",
    "/.vs/",
    "/.idea/",

    // Files
    "/[Dd]esktop.ini",
];

using var factory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = factory.CreateLogger<Program>();
var copyFilesCommand = new CopyFilesCommand(logger);

logger.LogInformation("Copying files ...");

copyFilesCommand.Execute(
    SourceDirectionPath,
    DestinationDirectionPath,
    ignorePatterns,
    SkipExistingDirectories,
    SkipExistingFiles);

logger.LogInformation(@"Copy completed \(^_^)/");