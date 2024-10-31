using FileFilterCopy.Commands;
using Microsoft.Extensions.Logging;

const string SourceDirectoryPath = @"D:\Programming projects";
const string DestinationDirectoryPath = @"F:\Programming projects";
const bool SkipExistingDirectories = false;
const bool SkipExistingFiles = false;

string[] ignorePatterns =
[
    // Paths
    "D:/Programming projects/.git/",
    "D:/Programming projects/.gitignore",
    "D:/Programming projects/C#/Ip Country Dataset/IpCountryDataset/Resources/**",

    // Exclude from ignoring
    "!D:/Programming projects/C#/Ip Country Dataset/IpCountryDataset/Resources/country.csv",

    // Directories
    "/[Bb]in/",
    "/[Oo]bj/",
    "/target/",
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
    SourceDirectoryPath,
    DestinationDirectoryPath,
    ignorePatterns,
    SkipExistingDirectories,
    SkipExistingFiles);

logger.LogInformation(@"Copy completed \(^_^)/");