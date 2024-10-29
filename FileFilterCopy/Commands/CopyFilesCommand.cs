using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace FileFilterCopy.Commands;

public interface ICopyFilesCommand
{
    void Execute(
        string sourceDirectionPath,
        string destinationDirectionPath,
        string[] ignorePatterns,
        bool skipExistingDirectories = false,
        bool skipExistingFiles = false);
}

public class CopyFilesCommand : ICopyFilesCommand
{
    private readonly ILogger _logger;

    public CopyFilesCommand(ILogger logger)
    {
        _logger = logger;
    }

    public void Execute(
        string sourceDirectionPath,
        string destinationDirectionPath,
        string[] ignorePatterns,
        bool skipExistingDirectories = false,
        bool skipExistingFiles = false)
    {
        var ignoreRegexes = ignorePatterns
            .Where(pattern => !string.IsNullOrWhiteSpace(pattern))
            .Select(pattern => new Regex(IgnorePatternToRegex(pattern)))
            .ToArray();

        CopyFiles(
            sourceDirectionPath,
            destinationDirectionPath,
            ignoreRegexes,
            skipExistingDirectories,
            skipExistingFiles);
    }

    private static string IgnorePatternToRegex(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return string.Empty;
        }

        return Regex.Escape(pattern)
            .Replace(@"\*\*", ".*") // ** any characters
            .Replace(@"\*", "[^/]*") // * any characters except /
            .Replace(@"\?", "[^/]") // ? one any character except /
            .Replace(@"\[", "[")
            .Replace(@"\]", "]");
    }

    private void CopyFiles(
        string sourceDirectionPath,
        string destinationDirectionPath,
        Regex[] ignoreRegexes,
        bool skipExistingDirectories,
        bool skipExistingFiles)
    {
        if (!Directory.Exists(sourceDirectionPath))
        {
            _logger.LogError("Source directory doesn't exist: {path}", sourceDirectionPath);
            return;
        }

        if (!Directory.Exists(destinationDirectionPath))
        {
            try
            {
                Directory.CreateDirectory(destinationDirectionPath);
                _logger.LogInformation("Destination directory has been created: {path}", destinationDirectionPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{message}", ex.Message);
                return;
            }
        }

        foreach (var file in Directory.GetFiles(sourceDirectionPath))
        {
            if (ShouldIgnore(file, ignoreRegexes)) continue;

            var destinationFilePath = Path.Combine(destinationDirectionPath, Path.GetFileName(file));

            if (skipExistingFiles && File.Exists(destinationFilePath)) continue;

            try
            {
                File.Copy(file, destinationFilePath, true);
                _logger.LogInformation("File copied to: {path}", destinationFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{message}", ex.Message);
            }
        }

        foreach (var directory in Directory.GetDirectories(sourceDirectionPath))
        {
            if (ShouldIgnore(directory, ignoreRegexes)) continue;

            var destinationSubdirectoryPath = Path.Combine(destinationDirectionPath, Path.GetFileName(directory));

            if (skipExistingDirectories && Directory.Exists(destinationSubdirectoryPath)) continue;

            CopyFiles(
                directory,
                destinationSubdirectoryPath,
                ignoreRegexes,
                skipExistingDirectories,
                skipExistingFiles);
        }
    }

    private static bool ShouldIgnore(string path, Regex[] ignoreRegexes)
    {
        var fileAttributes = File.GetAttributes(path);
        path = path.Replace("\\", "/");

        if (fileAttributes.HasFlag(FileAttributes.Directory) && !path.EndsWith('/'))
        {
            path += "/";
        }

        return ignoreRegexes.Any(regex => regex.IsMatch(path));
    }
}