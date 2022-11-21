module FsHtmlKit.AsPlainText

open FSharp.Data
open FSharp.Data.HtmlActivePatterns

module StringExt = FsCombinators.StringExtensions

open BufferManipulation

let elementsAndTextOnly = function
    | HtmlComment _ -> false
    | HtmlCData _ -> false
    | HtmlElement("script", _, _) -> false
    | HtmlElement("head", _, _) -> false
    | HtmlText _ -> true
    | HtmlElement(_, _, _) -> true

let asPlainText (node: HtmlNode) =
    match node with
    | HtmlElement(_, htmlAttributes, _)  ->
        htmlAttributes
        |> List.filter (fun attr -> StringExt.iequal (attr.Name ()) "href")
        |> List.map (fun attr -> sprintf "[%s]" (attr.Value()))
        |> String.concat " "
    | HtmlText textValue -> textValue
    | HtmlComment textValue -> textValue
    | HtmlCData textValue -> textValue
    |> replaceNbsp
