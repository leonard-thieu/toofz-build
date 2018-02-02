# toofz Build

[![Build status](https://ci.appveyor.com/api/projects/status/1ykql6v9dm8l9jfl/branch/master?svg=true)](https://ci.appveyor.com/project/leonard-thieu/toofz-build/branch/master)
[![codecov](https://codecov.io/gh/leonard-thieu/toofz-build/branch/master/graph/badge.svg)](https://codecov.io/gh/leonard-thieu/toofz-build)
[![MyGet](https://img.shields.io/myget/toofz/v/toofz.Build.svg)](https://www.myget.org/feed/toofz/package/nuget/toofz.Build)

## Installing via NuGet

Add a NuGet.Config to your solution directory with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="toofz" value="https://www.myget.org/F/toofz/api/v3/index.json" />
  </packageSources>
</configuration>
```

```powershell
Install-Package 'toofz.Build'
```

### Dependents

* [toofz](https://github.com/leonard-thieu/crypt.toofz.com)
* [toofz API](https://github.com/leonard-thieu/api.toofz.com)
* [toofz Leaderboards Service](https://github.com/leonard-thieu/leaderboards-service)
* [toofz Daily Leaderboards Service](https://github.com/leonard-thieu/daily-leaderboards-service)
* [toofz Players Service](https://github.com/leonard-thieu/players-service)
* [toofz Replays Service](https://github.com/leonard-thieu/replays-service)
* [toofz NecroDancer Core](https://github.com/leonard-thieu/toofz-necrodancer-core)
* [toofz Steam](https://github.com/leonard-thieu/toofz-steam)
* [toofz Data](https://github.com/leonard-thieu/toofz-data)
* [toofz Services Core](https://github.com/leonard-thieu/toofz-services-core)

## Requirements

* [.NET Standard 1.3](https://github.com/dotnet/standard/blob/master/docs/versions.md)-compatible platform
  * .NET Core 1.0
  * .NET Framework 4.6
  * Mono 4.6

## Contributing

Contributions are welcome for toofz Build.

* Want to report a bug or request a feature? [File a new issue](https://github.com/leonard-thieu/toofz-build/issues).
* Join in design conversations.
* Fix an issue or add a new feature.
  * Aside from trivial issues, please raise a discussion before submitting a pull request.

### Development

#### Requirements

* Visual Studio 2017

#### Getting started

Open the solution file and build. Use Test Explorer to run tests.

## License

**toofz Build** is released under the [MIT License](LICENSE).
