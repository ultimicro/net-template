# NetTemplate
[![Nuget](https://img.shields.io/nuget/v/NetTemplate)](https://www.nuget.org/packages/NetTemplate)

This is a fork of StringTemplate 4 for .NET to modernize it. The goal of this project is to be a superset of StringTemplate 4. That is, any template of StringTemplate 4 can be using with this library unmodified with additional features available.

## Usage

The 1.X version can be drop-in replacement for `StringTemplate4`. Just rename namespace from `Antlr4.StringTemplate` to `NetTemplate`. Every next major version will have some small breaking changes on the code so you can slowly upgraded. The template itself will never have a breaking change (unless StringTemplate 4 introduced it), only the code that using this library.

## Additional features

### Expression options

The following is a list of the new expression options in additional to StringTemplate 4 [expression options](https://github.com/antlr/stringtemplate4/blob/master/doc/expr-options.md):

- `culture`: specify the name of the culture to override the one that was specified in `Template.Render()` (e.g. `<foo; culture="th-TH">`). It can be any value that is acceptable by `CultureInfo.GetCultureInfo()`, even an empty string.

## Breaking changes

### 2.0 to 3.0

- All constructors of `NetTemplate.Interpreter` become internal.
- Any exception from the template will propagation to the call site instead of raising an INTERNAL_ERROR.

### 1.0 to 2.0

- `culture` parameter on `IAttributeRenderer.ToString()` will be able to override by the user.
- `IFormattable` object will be honored during rendering.
- `NetTemplate.Interpreter` become a sealed class.

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
