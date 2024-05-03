module FsHtmlKit.Spec.ExamplesSpec

open FsCheck
open FsCheck.Xunit

let newline = System.Environment.NewLine

[<Property>]
let ``E1.Complete HTML document with DOCTYPE declaration: only title`` () =
    let exampleText = System.IO.File.ReadAllText("examples/01.html")

    let expected: string option = Some $"Title: Cover letter and CV submission{newline}"
    let actual: string option = FsHtmlKit.Html2PlainText.Std.tryExtractPlainText exampleText

    actual = expected |> Prop.label (actual |> Option.defaultValue "Extraction failed")

[<Property>]
let ``E2.Complete HTML document with DOCTYPE declaration: minimal content`` () =
    let exampleText = System.IO.File.ReadAllText("examples/02.html")

    let expected: string option =
        Some <| $"Title: Cover letter and CV submission{newline}E02{newline}"

    let actual: string option = FsHtmlKit.Html2PlainText.Std.tryExtractPlainText exampleText

    actual = expected |> Prop.label (actual |> Option.defaultValue "Extraction failed")

[<Property>]
let ``E3.Invalid HTML document with orphan meta element: no content`` () =
    let exampleText = System.IO.File.ReadAllText("examples/03.html")

    let expected: string option = None

    let actual: string option = FsHtmlKit.Html2PlainText.Std.tryExtractPlainText exampleText

    actual = expected |> Prop.label (actual |> Option.defaultValue "Extraction failed")

[<Property>]
let ``E4.Invalid HTML document with orphan meta element: no content`` () =
    let exampleText = System.IO.File.ReadAllText("examples/26822503.html")

    let expected: string option = None

    let actual: string option = FsHtmlKit.Html2PlainText.Std.tryExtractPlainText exampleText

    actual = expected |> Prop.label (actual |> Option.defaultValue "Extraction failed")
