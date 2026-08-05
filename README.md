net-tools
=========

Net Industry repository/playground for any .NET reusable code.

[![CI](https://github.com/zidad/net-tools/actions/workflows/ci.yml/badge.svg)](https://github.com/zidad/net-tools/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Net.Core.svg)](https://www.nuget.org/packages/Net.Core)

Net.Core
-
Contains a set of .NET framework namespaces with no other dependencies, such as:

- CommandLine: abstractions for command line tasks
- Collections: mostly LINQ extension methods to make working with LINQ to objects easier
- DependencyInjection: generic attributes for dependency injection registration
- Enums: Easier Enum parsing/handling/attributing
- Reflection: Improve working with attributes
- System: DateTime/TimeSpan helpers and object conversion (`ToExtensions`)
- Text: String formatting and checking utilities

[install from nuget:](https://www.nuget.org/packages/Net.Core)

    dotnet add package Net.Core

Building
-

    dotnet build src/Net.Tools.sln
    dotnet test src/Net.Tools.sln

Releasing
-

Versions are derived from git tags by [MinVer](https://github.com/adamralph/minver) — there is no
version number to edit in the csproj.

- **Merging to `main`** publishes an auto-incremented prerelease, e.g. `0.2.0-alpha.0.91`.
- **Pushing a `v*` tag** publishes the matching stable version:

      git tag v0.3.0
      git push origin v0.3.0

  produces `Net.Core` `0.3.0`. Commits after that tag become `0.3.1-alpha.0.N`.

Publishing is handled by `.github/workflows/release.yml` and requires a `NUGET_API_KEY`
repository secret. Pushes use `--skip-duplicate`, so a merge that does not change the
version is a harmless no-op.

---

The MIT License (MIT)

Copyright (c) 2014 Wiebe Tijsma

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
