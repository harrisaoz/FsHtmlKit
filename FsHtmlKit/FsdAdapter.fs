module FsHtmlKit.FsdAdapter

open FSharp.Data
open FSharp.Data.HtmlActivePatterns

module N = NodeTypes
module H = HtmlRules.Headings
module Br = HtmlRules.Breaking
module Buf = BufferManipulation

let asNodeType (htmlNode: HtmlNode) =
    match htmlNode with
    | HtmlText text -> N.Text text
    | HtmlComment text -> N.Comment text
    | HtmlCData text -> N.CData text
    | HtmlElement ("script", _, _) -> N.ScriptElement
    | HtmlElement ("head", _, _) -> N.HeadElement
    | HtmlElement (name, attributes, _) ->
        N.Element(name, N.HtmlAttributes(attributes |> Seq.ofList))

let asVisitedNode (htmlNode: HtmlNode) =
    match htmlNode with
    | HtmlText textValue when textValue.Equals(Buf.newline) -> N.NewLineTextNode
    | HtmlElement (name, _, _) when (H.isHeading name) -> N.HeadingElement
    | HtmlElement (name, _, _) when Br.isBreakingElement name ->
        N.BreakingElement
    | _ -> N.OtherNode

let fsdLibAdapter =
    { new LibAdapter.HtmlLibAdapter<HtmlDocument, HtmlNode, HtmlAttribute> with
        member _.Parse(htmlText) = HtmlDocument.Parse htmlText

        member _.DocumentElements(document) =
            HtmlDocumentExtensions.Elements document

        member _.ChildNodes(node) =
            HtmlNodeExtensions.Elements node |> Seq.ofList

        member _.AsTransformableNode(node) = asNodeType node
        member _.AttributeName(attribute) = attribute.Name()
        member _.AttributeValue(attribute) = attribute.Value()
        member _.AsVisitedNode(node) = asVisitedNode node }
