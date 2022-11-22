# Purpose

Facilitate extraction of plain text from HTML documents.

# Behaviour

Use FSharp.Data to parse the HTML document into a tree structure, then a bespoke visitor to extract the desired text
elements. The simple façade that is the primary means of using the library has the following behaviour:

- comments are ignored
- CData nodes are ignored
- Attributes other than ```href``` are ignored
- Text nodes are passed through to the result
- Elements which should break a line in HTML rendering emit a new line in the result
- Runs of whitespace are collapsed into a single whitespace

# Usage

```f# script
#r "nuget: FsHtmlKit"
open FsHtmlKit.Html2PlainText

let inputHtml = System.IO.File.ReadAllText "input.html"

let ``Do something if extraction fails`` () = ...
let ``Do something with the plain text`` plainText = ...
let ``Do something with text that doesn't care whether the result is html or plain text`` text = ...

tryExtractPlainText inputHtml |> function
    | None -> ``Do something if extraction fails`` ()
    | Some plainText -> ``Do something with the plain text`` plainText

html2Text inputHtml
|> ``Do something with text that doesn't care whether the result is html or plain text``
```

# Build (for Release)

```powershell
dotnet build -c Release
```

# Package

```powershell
dotnet pack -c Release
```

# Deploy

```powershell
dotnet nuget push FsHtmlKit\bin\Release\FsHtmlKit.<version>.nupkg -s <github-source-name> -k <github-package-deployment-api-key>
```
