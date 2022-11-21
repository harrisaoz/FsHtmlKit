module FsHtmlKit.Spec.AsPlainTextSpec

open FSharp.Data
open FsCheck
open FsCheck.Xunit

open FsHtmlKit.AsPlainText
open FSharp.Data.HtmlActivePatterns

module Htg = HtmlTextGen
open Giraffe.ViewEngine

let htmlEncode text =
    RenderView.AsString.htmlNode <| str text

let arbHtmlText =
    Arb.Default.String().Generator
    |> Gen.map HtmlTextGen.create
    |> Gen.filter Option.isSome
    |> Arb.fromGen

let textMatches example =
    Prop.forAll arbHtmlText
    <| fun m ->
        let text = Option.get m |> HtmlTextGen.value

        asPlainText (example text) = text
        |> Prop.collect text.Length

[<Property>]
let ``Inverse behaviour for text nodes`` () = textMatches HtmlNode.NewText

[<Property>]
let ``Inverse behaviour for comments`` () = textMatches HtmlNode.NewComment

[<Property>]
let ``Inverse behaviour for CData`` () = textMatches HtmlNode.NewCData

[<Property>]
let ``Anchor URLs should be delimited by [] in the result`` () =
    let example url =
        HtmlNode.NewElement("a", [ "href", url ], [])

    Prop.forAll arbHtmlText
    <| fun m ->
        let text = Option.get m |> HtmlTextGen.value
        asPlainText (example text) = $"[{text}]"

[<Property>]
let ``Text nodes are accepted`` (node: HtmlNode) =
    match node with
    | HtmlText _ -> elementsAndTextOnly node = true
    | _ -> true

[<Property>]
let ``Elements that are neither script nor head elements are accepted`` (node: HtmlNode) =
    match node with
    | HtmlElement (name, _, _) when not (List.contains name [ "script"; "head" ]) -> elementsAndTextOnly node = true
    | _ -> true

[<Property>]
let ``Elements that are either script or head elements are rejected`` (node: HtmlNode) =
    match node with
    | HtmlElement (name, _, _) when (List.contains name [ "script"; "head" ]) -> elementsAndTextOnly node = false
    | _ -> true

[<Property>]
let ``Comment nodes are rejected`` (node: HtmlNode) =
    match node with
    | HtmlComment _ -> elementsAndTextOnly node = false
    | _ -> true

[<Property>]
let ``CData nodes are rejected`` (node: HtmlNode) =
    match node with
    | HtmlCData _ -> elementsAndTextOnly node = false
    | _ -> true
