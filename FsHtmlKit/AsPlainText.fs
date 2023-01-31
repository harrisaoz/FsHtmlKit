module FsHtmlKit.AsPlainText

open FsHtmlKit.NodeTypes

module StringExt = FsCombinators.StringExtensions

open BufferManipulation
module N = NodeTypes

let elementsAndTextOnly (asNodeType: 'n -> N.HtmlNode<'a>) (node: 'n) =
    match (asNodeType node) with
    | Comment _ -> false
    | CData _ -> false
    | ScriptElement -> false
    | HeadElement -> false
    | Text _ -> true
    | Element _ -> true

let asPlainText asNodeType attrName attrValue node =
    match (asNodeType node) with
    | Text text -> text
    | Comment text -> text
    | CData text -> text
    | Element (_, HtmlAttributes attributes) ->
        attributes
        |> Seq.filter (fun attr -> StringExt.iequal (attrName attr) "href")
        |> Seq.map (fun attr -> sprintf "[%s]" (attrValue attr))
        |> String.concat " "
    | _ -> ""
    |> replaceNbsp
