# dotnet-handle

A tool to identify which process has a file or directory handle locked, and optionally kill the process.
> Note that this tool works by downloading Microsoft SysInternals handle.exe and parsing it's output.

**To install**
```
dotnet tool install -g dotnet-handle
```

**To use**
```
dotnet handle [directoryPath]
dotnet handle [filePath]
```