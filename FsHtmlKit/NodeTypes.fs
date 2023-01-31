module FsHtmlKit.NodeTypes

type HtmlAttributes<'a> = HtmlAttributes of attributes: 'a seq

type HtmlNode<'a> =
    | Comment of text: string
    | CData of text: string
    | Text of text: string
    | ScriptElement
    | HeadElement
    | Element of name: string * attributes: HtmlAttributes<'a>

type VisitedNode =
    | NewLineTextNode
    | HeadingElement
    | BreakingElement
    | OtherNode
