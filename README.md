# NetTemplate
[![Nuget](https://img.shields.io/nuget/v/NetTemplate)](https://www.nuget.org/packages/NetTemplate)

This is a fork of StringTemplate 4 for .NET to modernize it. The goal of this project is to be a superset of StringTemplate 4. That is, backward compatible with
StringTemplate 4 with additional features.

## Usage

The current version can be drop-in replacement for `StringTemplate4`. Just rename namespace from `Antlr4.StringTemplate` to `NetTemplate`.

## Additional features

### Expression options

The following is a list of the new expression options in additional to StringTemplate 4 [expression options](https://github.com/antlr/stringtemplate4/blob/master/doc/expr-options.md):

- `culture`: specify the name of the culture to override the one that was specified in `Template.Render()` (e.g. `<foo; culture="th-TH">`). It can be any value that is acceptable by `CultureInfo.GetCultureInfo()`, even an empty string.

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
