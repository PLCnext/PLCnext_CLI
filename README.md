# Welcome to the PLCnext CLI main repository

The PLCnext CLI (PLCnext Command Line Interface) provides the entire toolchain for C++ programming on the PLCnext Technology platform as well as a [template system](link_zum_Template_System) for creating projects. Based on the templates, you can develop your applications. Use the PLCnext CLI to unpack and manage the SDKs. CMake is contained as the build environment and each SDK has its own configuration. A parser integrated in PLCnext CLI creates the metadata required for PLCnext Technology. The LibraryBuilder contained in PLCnext CLI creates a PLCnext Engineer li­brary from the project. For convinience you can also use on of our IDE plugins. They are part of the release version or can also be found here on GitHub for [Visual Studio]() and [Eclipse]().

## Installation & First Steps

If you want the latest release version, you can download it from the the [PLCnext Technology controller download area](https://www.phoenixcontact.com/qr/2404267/softw).
For more information and first steps with PLCnext Technology please visit our [PLCnext Community](https://www.plcnext-community.net/en/hn-knowledge-base/hn-get-started/hn-get-started-programming.html).

## Test a local build

If you want to adapt the PLCnext CLI to your needs or just work with the newest features and fixes. The following steps describe how to build a fully functional PLCnext CLI locally on your machine.

### Prerequisite

- [.NET Core 3.X](https://dotnet.microsoft.com/download/dotnet-core ".NET Core 3.X")

### External tools to download

- [CMake as an archive](https://cmake.org/download/ "CMake as an archive")
- Engineering Library Builder extracted from the latest [PLCnext CLI Installation] (https://www.phoenixcontact.com/qr/2404267/softw "PLCnext CLI Setup download page")

### Build steps

- `dotnet build src -p:PlcNextToolName=plcncli`
- Extract cmake to `src/PlcNext/bin/Debug/netcoreapp3.0/cmake` so that the `bin` folder can be found directly in `src/PlcNext/bin/Debug/netcoreapp3.0/cmake`
- Extract the Engineering Library Builder to `src/PlcNext/bin/Debug/netcoreapp3.0/library-builder`
- Edit the `src/PlcNext/bin/Debug/netcoreapp3.0/file-names.xml` file to look like:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<names>
  <application path="plcncli"></application>
  <EngineeringLibraryBuilder path="library-builder/EngineeringLibraryBuilder"></EngineeringLibraryBuilder>
  <cmake path="cmake/bin/cmake"></cmake>
</names>
```

- Use `src/PlcNext/bin/Debug/netcoreapp3.0/plcncli` e.g. but add it to the path environment variable

## Contribute

Currently not supported. We are working on a process for contribution.

## Feedback

You can give feedback to this project in different ways:

- Ask a question in our [Forum](https://www.plcnext-community.net/index.php?option=com_easydiscuss&view=categories&Itemid=221&lang=en).
- Request a new feature via [GitHub Issues](https://github.com/PLCnext/PLCnext_CLI/issues).
- Vote for [Popular Feature Requests](https://github.com/PLCnext/PLCnext_CLI/issues?q=is%3Aopen+is%3Aissue+label%3Afeature-request+sort%3Areactions-%2B1-desc).
- File a bug in [GitHub Issues](https://github.com/PLCnext/PLCnext_CLI/issues).

## License

Copyright (c) Phoenix Contact Gmbh & Co KG. All rights reserved.
Licensed under the [Apache-2.0](LICENSE) License.