@echo off

dotnet build ./src
dotnet test .\src\MCompiler.Tests\MCompiler.Tests.csproj