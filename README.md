![System.IO.Abstractions.Extensions](https://socialify.git.ci/System-IO-Abstractions/System.IO.Abstractions.Extensions/image?description=1&font=Source%20Code%20Pro&forks=1&issues=1&pattern=Charlie%20Brown&pulls=1&stargazers=1&theme=Dark)
[![NuGet](https://img.shields.io/nuget/v/TestableIO.System.IO.Abstractions.Extensions.svg)](https://www.nuget.org/packages/TestableIO.System.IO.Abstractions.Extensions)
![Continuous Integration](https://github.com/TestableIO/System.IO.Abstractions.Extensions/workflows/Continuous%20Integration/badge.svg)
[![Renovate enabled](https://img.shields.io/badge/renovate-enabled-brightgreen.svg)](https://renovatebot.com/)
<!-- [![Codacy Badge](https://api.codacy.com/project/badge/Grade/2e777fa545c94767acccd6345b1ed9b7)](https://app.codacy.com/gh/TestableIO/System.IO.Abstractions.Extensions?utm_source=github.com&utm_medium=referral&utm_content=TestableIO/System.IO.Abstractions.Extensions&utm_campaign=Badge_Grade_Dashboard) -->
<!-- [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FTestableIO%2FSystem.IO.Abstractions.Extensions.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FTestableIO%2FSystem.IO.Abstractions.Extensions?ref=badge_shield) -->

Convenience functionality on top of System.IO.Abstractions

```shell
dotnet add package TestableIO.System.IO.Abstractions.Extensions
```

# Examples

## CurrentDirectory extension

```csharp
var fs = new FileSystem();

//with extension
var current = fs.CurrentDirectory();

//without extension
var current =  fs.DirectoryInfo.New(fs.Directory.GetCurrentDirectory());
```

## SubDirectory extension

```csharp
var current = new FileSystem().CurrentDirectory();

//create a "temp" subdirectory with extension
current.SubDirectory("temp").Create();

//create a "temp" subdirectory without extension
current.FileSystem.DirectoryInfo.New(current.FileSystem.Path.Combine(current.FullName, "temp")).Create();
```

## File extension

```csharp
var current = new FileSystem().CurrentDirectory();

//create a "test.txt" file with extension
using (var stream = current.File("test.txt").Create())
    stream.Dispose();

//create a "test.txt" file without extension
using (var stream = current.FileSystem.FileInfo.New(current.FileSystem.Path.Combine(current.FullName, "test.txt")).Create())
    stream.Dispose();
```

## IDirectoryInfo.CopyTo extension
```csharp
var fs = new FileSystem();
var current = fs.CurrentDirectory();

//source
var source =  fs.DirectoryInfo.New(fs.Path.Combine(current.FullName, "SourceDir"));

//destination
var dest =  fs.DirectoryInfo.New(fs.Path.Combine(current.FullName, "DestDir"));

//copy
source.CopyTo(dest, recursive: true);
```
