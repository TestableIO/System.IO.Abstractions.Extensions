# Release Notes

List of notable changes between majors

## 22.0.0

- (Breaking) Removed support for some Async calls in .NET Framework and
  legacy versions of .NET to better support .NET 8 and later
- Minimum required TestableIO.System.IO.Abstractions version is now 22.x
- Removed .NET 7 build (should still work with .NET standard build)
- Removed .NET 6 build (should still work with .NET standard build)

## 2.0.0

- (Breaking) Moved all extensions methods to 'System.IO.Abstractions' namespace
- Added ThrowIfNotFound extension methods
- Removed .NET 5 build (should still work with .NET standard build)