# FileFilterCopy

File copying with filtering

## Description

This application copies files and directories from a source path to a destination path, with optional filtering based on custom patterns. It allows you to specify paths to copy from and to, and provides options to skip copying or overwriting existing files and directories.

## Pattern Syntax

The application supports the following pattern symbols:

| Pattern | Description                                                            |
| ------- | :--------------------------------------------------------------------- |
| \*\*    | Matches one or more of any character.                                  |
| \*      | Matches any character except for a forward slash (`/`).                |
| ?       | Matches any single character except for a forward slash (`/`).         |
| []      | Matches any one character from a specified set.                        |
| !       | When used at the start of a pattern, it exclude other ignore patterns. |

> Forward slash (`/`) at the beginning of a pattern is used to avoid matches in the middle of file names or at the end of directory names.
>
> Forward slash (`/`) at the end of a pattern indicates that the pattern should only be applied to directories.

### Usage Examples

- Paths
  - D:/your/directory/names/.git/
  - D:/your/directory/names/.gitignore
  - D:/your/directory/names/IpCountryDataset/Resources/\*\*
- Exclude from ignoring
  - !D:/your/directory/names/IpCountryDataset/Resources/country.csv
- Directories
  - /\[Bb\]in/
  - /\[Oo\]bj/
  - /target/
  - /node_modules/
  - /.vs/
  - /.idea/
- Files
  - /\[Dd\]esktop.ini

## References

- [Git ignore](https://www.atlassian.com/git/tutorials/saving-changes/gitignore)
