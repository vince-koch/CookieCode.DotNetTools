rmdir publish /s /q
rmdir nuget /s /q
dotnet publish -o publish/
dotnet pack --no-restore --no-build -o nuget/ /p:OutputPath=publish