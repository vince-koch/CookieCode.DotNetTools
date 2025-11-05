# CookieCode.DotNetTools

## Quick Start (local)

```powershell
dotnet clean
dotnet build -c Release
dotnet pack -c Release

dotnet tool install -g CookieCode.DotNetTools --add-source .\CookieCode.DotNetTools\bin\Nupkg
dotnet tool update -g CookieCode.DotNetTools --add-source .\CookieCode.DotNetTools\bin\Nupkg
```

[CookieCode.DotNetTools](CookieCode.DotNetTools/ReadMe.md) is a set of console tools with a variety of uses.

![Cookie](cookie.png)
