# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unpublished/2.1.5]
### Changed
- Bump Microsoft.NET.Test.Sdk from 17.9.0 to 17.10.0. Only affects the test project.
- Bump xunit from 2.7.1 to 2.9.0. Only affects the test project.
- Bump xunit.runner.visualstudio from 2.5.7 to 2.8.2. Only affects the test project.

## [Unpublished]
### Changed
- Bump Microsoft.NET.Test.Sdk from 17.8.0 to 17.9.0. Only affects the test project.
- Bump xunit from 2.6.6 to 2.7.1. Only affects the test project.
- Bump xunit.runner.visualstudio from 2.5.6 to 2.5.7. Only affects the test project.
- Bump coverlet.collector from 6.0.0 to 6.0.2. Only affects the test vocerage report generator.

## [2.1.4]
### Changed
- Updated the README.md and Console Program to make it clear what the arguments of activities are. [Issue #47](https://github.com/NovoNordisk-OpenSource/critical-path/issues/47)

### Fixed
- Fixed performance issue with large activity graphs [Issue #48](https://github.com/NovoNordisk-OpenSource/critical-path/issues/48)

## [2.1.3] - 2024-01-23
### Fixed
- Improved performance for large activity graphs [Issue #43](https://github.com/NovoNordisk-OpenSource/critical-path/issues/43)

## [2.1.2] - 2024-01-19
### Added
- Readme badges
- XML docs for IDE IntelliSense 

### Changed
- Bump xunit from 2.6.5 to 2.6.6 in the test project

## [2.1.1] - 2024-01-12
### Changed
- Make the nuget package deterministic

## [2.1.0] - 2024-01-12
### Added
- Symbols for easy debugging by consumers of this library

### Changed
- The release note tab now reference the GitHub release page
- Bump xunit from 2.6.3 to 2.6.5 in the test project
- Bump xunit.runner.visualstudio from 2.5.5 to 2.5.6

## [2.0.0] - 2023-12-20
### Added
- Initial library added to nuget.org and metadata that goes with it.
- Support for multiple target frameworks 
  -   netstandard 2.0
  -   netstandard 2.1
  -   dotnet 6
  -   dotnet 8
