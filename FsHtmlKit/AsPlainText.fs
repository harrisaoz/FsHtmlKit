module FsHtmlKit.AsPlainText

open FsHtmlKit.NodeTypes

module StringExt = FsCombinators.StringExtensions

open BufferManipulation
module N = NodeTypes

let elementsAndTextOnly (asNodeType: 'n -> N.HtmlNode<'a>) (node: 'n) =
    let nodeType = asNodeType node
    match nodeType with
    | Comment _ -> false
    | CData _ -> false
    | ScriptElement -> false
    | HeadElement -> true
    | IgnorableChildOfHead -> false
    | TitleElement -> true
    | Text _ -> true
    | Element _ -> true

let asPlainText asNodeType attrName attrValue node =
    let nodeType = asNodeType node
    match nodeType with
    | Text text -> text
    | Comment text -> text
    | TitleElement -> "Title: "
    | CData text -> text
    | Element (_, HtmlAttributes attributes) ->
        attributes
        |> Seq.filter (fun attr -> StringExt.iequal (attrName attr) "href")
        |> Seq.map (fun attr -> sprintf "[%s]" (attrValue attr))
        |> String.concat " "
    | _ -> ""
    |> replaceNbsp
