module FsHtmlKit.HapAdapter

open FsHtmlKit.LibAdapter
open HtmlAgilityPack

module Br = HtmlRules.Breaking
module Sp = HtmlRules.Space
module H = HtmlRules.Headings
module Buf = BufferManipulation

module N = NodeTypes

let asNodeType (node: HtmlNode) =
    let isCData (node: HtmlNode) = HtmlNode.IsCDataElement(node.Name)

    let isScriptElement (node: HtmlNode) = node.Name = "script"

    let isHeadElement (node: HtmlNode) = node.Name = "head"

    match node with
    | :? HtmlTextNode as textNode -> N.Text textNode.Text
    | :? HtmlCommentNode as commentNode -> N.Comment commentNode.Comment
    | _ when isCData node -> N.CData node.InnerText
    | _ when isScriptElement node -> N.ScriptElement
    | _ when isHeadElement node -> N.HeadElement
    | _ -> N.Element(node.Name, N.HtmlAttributes(node.Attributes))

let asVisitedNode (node: HtmlNode) =
    let isBreakingElement (node: HtmlNode) =
        node.NodeType = HtmlNodeType.Element && Br.isBreakingElement node.Name

    match node with
    | :? HtmlTextNode as textNode when textNode.Text.Equals(Buf.newline) ->
        N.NewLineTextNode
    | _ when node.NodeType = HtmlNodeType.Element && H.isHeading node.Name ->
        N.HeadingElement
    | _ when isBreakingElement node -> N.BreakingElement
    | _ -> N.OtherNode

let hapLibAdapter =
    { new HtmlLibAdapter<HtmlDocument, HtmlNode, HtmlAttribute> with
        member _.Parse(htmlText) =
            let doc = HtmlDocument()
            doc.LoadHtml(htmlText)
            doc

        member _.DocumentElements(document) = document.DocumentNode.ChildNodes
        member _.ChildNodes(node) = node.ChildNodes
        member _.AsTransformableNode(node) = asNodeType node
        member _.AttributeName(attribute) = attribute.Name
        member _.AttributeValue(attribute) = attribute.Value
        member _.AsVisitedNode(node) = asVisitedNode node }
