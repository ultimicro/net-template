# NetTemplate

This is a fork of StringTemplate 4 for .NET to modernize it. The goal of this project is to be a superset of StringTemplate 4. That is, backward compatible with
StringTemplate 4 with additional features.

## Usage

The current version can be drop-in replacement for `StringTemplate4`. Just rename namespace from `Antlr4.StringTemplate` to `NetTemplate`.

## Development

### Prerequisites

- .NET 6 SDK

### Build

```sh
dotnet build src/NetTemplate.sln
```

### Run tests

```sh
dotnet test src/NetTemplate.sln
```

## License

BSD 3-Clause
