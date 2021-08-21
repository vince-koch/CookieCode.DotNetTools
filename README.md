# DotNetTools

A collection of simple tools to make life just a little better

## dotnet-handle

A tool to identify which process has a file or directory handle locked, and optionally kill the process.
Note that this tool works by downloading Microsoft SysInternals handle.exe and parsing it's output.

**To install**
```
dotnet tool install dotnet-handle
```

**To use**
```
dotnet handle [directoryPath]
dotnet handle [filePath]
```

## dotnet-prune

Removes all empty directories in a folder

**To install**
```
dotnet tool install dotnet-prune
```

**To use**
```
dotnet prune [directory]

Arguments:
    directory     The directory to begin pruning at.
                  Defaults to the current working directory if not provided.
```