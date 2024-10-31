using System.Text.RegularExpressions;

namespace FileFilterCopy.Helpers;

public class PathFilter
{
    protected List<Regex> ignorePathRegexes = [];
    protected List<Regex> excludePathRegexes = [];

    public PathFilter(IEnumerable<string> patterns)
    {
        InitializeRegexes(patterns);
    }

    private void InitializeRegexes(IEnumerable<string> patterns)
    {
        foreach (var pattern in patterns)
        {
            if (string.IsNullOrWhiteSpace(pattern)) continue;

            var patternEscape = GetPatternEscape(pattern);

            if (patternEscape.StartsWith('!'))
            {
                patternEscape = patternEscape.Substring(1);
                excludePathRegexes.Add(new Regex(patternEscape));
            }
            else
            {
                ignorePathRegexes.Add(new Regex(patternEscape));
            }
        }
    }

    private static string GetPatternEscape(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return string.Empty;
        }

        return Regex.Escape(pattern)
            .Replace(@"\*\*", ".*") // ** one or more of any character
            .Replace(@"\*", "[^/]*") // * any characters except forward slash
            .Replace(@"\?", "[^/]") // ? any single character except forward slash
            .Replace(@"\[", "[")
            .Replace(@"\]", "]");
    }

    public bool ShouldIgnoreDirectoryPath(string path)
    {
        if (!path.EndsWith('/') && !path.EndsWith('\\'))
        {
            path += "/";
        }

        return ShouldIgnorePath(path);
    }

    public bool ShouldIgnoreFilePath(string path)
    {
        return ShouldIgnorePath(path);
    }

    private bool ShouldIgnorePath(string path)
    {
        path = path.Replace("\\", "/");

        foreach (var regex in excludePathRegexes)
        {
            if (regex.IsMatch(path))
            {
                return false;
            }
        }

        foreach (var regex in ignorePathRegexes)
        {
            if (regex.IsMatch(path))
            {
                return true;
            }
        }

        return false;
    }
}
