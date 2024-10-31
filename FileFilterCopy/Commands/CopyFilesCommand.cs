using FileFilterCopy.Helpers;
using Microsoft.Extensions.Logging;

namespace FileFilterCopy.Commands;

public interface ICopyFilesCommand
{
    void Execute(
        string sourceDirectoryPath,
        string destinationDirectoryPath,
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
        string sourceDirectoryPath,
        string destinationDirectoryPath,
        string[] ignorePatterns,
        bool skipExistingDirectories = false,
        bool skipExistingFiles = false)
    {
        var pathFilter = new PathFilter(ignorePatterns);

        CopyFiles(
            sourceDirectoryPath,
            destinationDirectoryPath,
            pathFilter,
            skipExistingDirectories,
            skipExistingFiles);
    }

    private void CopyFiles(
        string sourceDirectoryPath,
        string destinationDirectoryPath,
        PathFilter pathFilter,
        bool skipExistingDirectories,
        bool skipExistingFiles)
    {
        if (!Directory.Exists(sourceDirectoryPath))
        {
            _logger.LogError("Source directory doesn't exist: {path}", sourceDirectoryPath);
            return;
        }

        // if path is not ignored and directory creation failed
        if (!pathFilter.ShouldIgnoreDirectoryPath(sourceDirectoryPath) &&
            !TryCreateDestinationDirectory(destinationDirectoryPath))
        {
            return;
        }

        foreach (var srcFilePath in Directory.GetFiles(sourceDirectoryPath))
        {
            if (pathFilter.ShouldIgnoreFilePath(srcFilePath)) continue;

            var destinationFilePath = Path.Combine(destinationDirectoryPath, Path.GetFileName(srcFilePath));

            if (skipExistingFiles && File.Exists(destinationFilePath)) continue;

            CopyFile(srcFilePath, destinationFilePath);
        }

        foreach (var srcDirectoryPath in Directory.GetDirectories(sourceDirectoryPath))
        {
            var destinationSubdirectoryPath = Path.Combine(
                destinationDirectoryPath,
                Path.GetFileName(srcDirectoryPath));

            if (skipExistingDirectories && Directory.Exists(destinationSubdirectoryPath)) continue;

            CopyFiles(
                srcDirectoryPath,
                destinationSubdirectoryPath,
                pathFilter,
                skipExistingDirectories,
                skipExistingFiles);
        }
    }

    private bool TryCreateDestinationDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return true;
        }

        try
        {
            Directory.CreateDirectory(path);
            _logger.LogInformation("Destination directory has been created: {path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{message}", ex.Message);
            return false;
        }

        return true;
    }

    private void CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite = true)
    {
        try
        {
            // create all directories for the file if they do not exist
            Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath) ?? string.Empty);

            File.Copy(sourceFilePath, destinationFilePath, overwrite);
            _logger.LogInformation("File copied to: {path}", destinationFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{message}", ex.Message);
        }
    }
}