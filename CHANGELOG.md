# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.3.0] - 2026-05-06

### Changed
- Normalize license expiration to UTC and compare calendar dates only, preventing time-zone and time-of-day expiration errors.

### Added
- Support for .NET Framework 4.8.1.

## [1.2.2] - 2026-01-04

### Added
- Support for .NET 10.

## [1.2.1] - 2024-11-14

### Added
- Support for .NET 9.

## [1.2.0] - 2024-07-31

### Changed
- Replaced bundled BouncyCastle sources with the `BouncyCastle.Cryptography` package.
- Limited supported target frameworks to .NET 6+, .NET Standard 2.0, and .NET Framework 4.6.1.

## [1.1.9] - 2024-06-09

### Added
- Allow callers to supply a custom date when validating license expiration.

### Fixed
- Avoid multiple enumeration during `AssertValidLicense` validation.

## [1.1.8] - 2024-04-27

### Fixed
- Correct validation-chain behavior for validation lists.

## [1.1.7] - 2024-01-04

### Changed
- Updated BouncyCastle dependency support and included the test project.

## [1.1.6] - 2024-01-04

### Added
- Support for .NET 6 and .NET 8.

### Removed
- Portable Class Library target support.

## [1.1.5] - 2018-02-17

### Changed
- Migrated the library and tests to the `Standard.Licensing` .NET Standard project structure.

[unreleased]: https://github.com/junian/Standard.Licensing/compare/v1.3.0...HEAD
[1.3.0]: https://github.com/junian/Standard.Licensing/compare/v1.2.2...v1.3.0
[1.2.2]: https://github.com/junian/Standard.Licensing/compare/v1.2.1...v1.2.2
[1.2.1]: https://github.com/junian/Standard.Licensing/compare/91d1a99181b1927446729a5ec04993ca41fb05a0...v1.2.1
[1.2.0]: https://github.com/junian/Standard.Licensing/compare/v1.1.9...91d1a99181b1927446729a5ec04993ca41fb05a0
[1.1.9]: https://github.com/junian/Standard.Licensing/compare/4788d1deed072814fa3d887eeb67130755bd6f8a...v1.1.9
[1.1.8]: https://github.com/junian/Standard.Licensing/compare/v1.1.7...4788d1deed072814fa3d887eeb67130755bd6f8a
[1.1.7]: https://github.com/junian/Standard.Licensing/compare/v1.1.6...v1.1.7
[1.1.6]: https://github.com/junian/Standard.Licensing/compare/v1.1.5...v1.1.6
[1.1.5]: https://github.com/junian/Standard.Licensing/releases/tag/v1.1.5
