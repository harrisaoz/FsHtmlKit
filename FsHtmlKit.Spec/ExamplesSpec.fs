module FsHtmlKit.Spec.ExamplesSpec

open FsCheck
open FsCheck.Xunit

[<Property>]
let ``E1.Complete HTML document with DOCTYPE declaration: no content`` () =
    let exampleText = System.IO.File.ReadAllText("examples/01.html")

    let expected: string option = None
    let actual: string option = FsHtmlKit.Html2PlainText.Std.tryExtractPlainText exampleText

    actual = expected

[<Property>]
let ``E2.Complete HTML document with DOCTYPE declaration: minimal content`` () =
    let exampleText = System.IO.File.ReadAllText("examples/02.html")

    let expected: string option = Some <| $"E02{System.Environment.NewLine}"
    let actual: string option = FsHtmlKit.Html2PlainText.Std.tryExtractPlainText exampleText

    actual = expected |> Prop.label(actual |> Option.defaultValue "Extraction failed")
