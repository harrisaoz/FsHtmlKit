module FsHtmlKit.LibAdapter

open FsHtmlKit.NodeTypes

type HtmlLibAdapter<'Document, 'Node, 'Attr> =
    abstract member Parse: string -> 'Document
    abstract member DocumentElements: 'Document -> 'Node seq
    abstract member ChildNodes: 'Node -> 'Node seq
    abstract member AsTransformableNode: 'Node -> HtmlNode<'Attr>
    abstract member AttributeName: 'Attr -> string
    abstract member AttributeValue: 'Attr -> string
    abstract member AsVisitedNode: 'Node -> VisitedNode

let parse (adapter: HtmlLibAdapter<'D, 'N, 'A>) = adapter.Parse

let documentElements (adapter: HtmlLibAdapter<'D, 'N, 'A>) _ =
    adapter.DocumentElements

let childNodes (adapter: HtmlLibAdapter<'D, 'N, 'A>) = adapter.ChildNodes

let asTransformableNode (adapter: HtmlLibAdapter<'D, 'N, 'A>) =
    adapter.AsTransformableNode

let attributeName (adapter: HtmlLibAdapter<'D, 'N, 'A>) = adapter.AttributeName

let attributeValue (adapter: HtmlLibAdapter<'D, 'N, 'A>) =
    adapter.AttributeValue

let asVisitedNode (adapter: HtmlLibAdapter<'D, 'N, 'A>) = adapter.AsVisitedNode
