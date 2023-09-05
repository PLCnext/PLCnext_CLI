# Welcome to the PLCnext CLI main repository

The PLCnext CLI (PLCnext Command Line Interface) provides the entire toolchain for C++ programming on the PLCnext Technology platform as well as a [template system](https://github.com/PLCnext/PLCnext_CLI_Templates) for creating projects. Based on the templates, you can develop your applications.<br/>
Use the PLCnext CLI to unpack and manage the SDKs. CMake is contained as the build environment and each SDK has its own configuration.<br/>
A parser integrated in PLCnext CLI creates the metadata required for PLCnext Technology.<br/>
The LibraryBuilder contained in PLCnext CLI creates a PLCnext Engineer li­brary from the project.

For convenience you can also use one of our IDE plugins. They are part of the release version or can also be found here on GitHub for [Visual Studio](https://github.com/PLCnext/PLCnext_CLI_VS/tree/master) and [Eclipse](https://github.com/PLCnext/PLCnext_CLI_Eclipse/tree/master).

## Installation & First Steps

If you want the latest release version, you can download it from the the [PLCnext toolchain product page](https://www.phoenixcontact.com/qr/1639782).<br/>
For more information and first steps with PLCnext Technology please visit our [PLCnext Community](https://www.plcnext-community.net/infocenter/programming/plcnext-programming_introduction).

## Test a local build

If you want to adapt the PLCnext CLI to your needs or just work with the newest features and fixes. The following steps describe how to build a fully functional PLCnext CLI locally on your machine.

### Prerequisite

- [.NET 7](https://dotnet.microsoft.com/en-us/download/dotnet)

### External tools to download

- [CMake as an archive](https://cmake.org/download/)
- Engineering Library Builder extracted from the latest [PLCnext toolchain product page](https://www.phoenixcontact.com/qr/1639782)

### Build steps

- `dotnet build src -p:PlcNextToolName=plcncli -p:RestoreConfigFile=src/NuGet/LocalFeed.config`
- Extract cmake to `src/PlcNext/bin/Debug/net7.0/cmake`
- Extract the Engineering Library Builder to `src/PlcNext/bin/Debug/net7.0/library-builder`
- Edit the `src/PlcNext/bin/Debug/net7.0/file-names.xml` file to look like:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<names>
  <application path="plcncli"></application>
  <EngineeringLibraryBuilder path="library-builder/EngineeringLibraryBuilder"></EngineeringLibraryBuilder>
  <cmake path="cmake/bin/cmake"></cmake>
</names>
```

- Use finally `src/PlcNext/bin/Debug/net7.0/plcncli.exe`.<br/>
  Consider to add a search path to the path environment variable.

## Contribute

You can participate in this project by submitting bugs and feature requests.<br/>
Furthermore you can help us by discussing issues and let us know where you have problems or where others could struggle.

## Feedback

You can give feedback to this project in different ways:

- Ask a question in our [PLCnext Community Forum](https://www.plcnext-community.net/forum).
- File a bug or request a new feature in [GitHub Issues](https://github.com/PLCnext/PLCnext_CLI/issues).

## License

Copyright (c) Phoenix Contact GmbH & Co KG. All rights reserved.<br/>

Licensed under the [Apache 2.0](LICENSE) License.