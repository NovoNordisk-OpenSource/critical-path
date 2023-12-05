# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- Description in `README.md` about how to consume library from the Digital Foundation nuget feed. 
- Esage example in `README.md`

## [1.1.0] - 2023-04-04
### Added
- Added an `ICriticalPathMethod` interface.

## [1.0.2] - 2023-04-04
### Changed
- The `Id` in `Activity`is no longer nullable. Instead 0 is used as the default value.

## [1.0.1] - 2023-04-03
### Added
- Added an optional `long Id` in the `Activity` 

### Changed
- Renamed the class `CriticalPath` to `CriticalPathMethod`
- Made `CriticalPathMethod` non-static
- Made `Name` optional in the `Activity`

### Deprecated
- `Id` in the `Àctivity` is deprecated. It will be removed later and replaced by a way to add an entire object to the Activity. 

## [0.0.1] - 2023-03-30
### Added
- Initial library
