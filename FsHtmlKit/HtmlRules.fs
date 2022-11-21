module FsHtmlKit.HtmlRules

module Headings =
    let isHeading (elementName: string) =
        System.Text.RegularExpressions.Regex("h[1-6]").IsMatch(elementName.ToLower())

module Space =
    let collapsibleSpace = [|
        '\u0020'
        '\u0009'
        '\u000C'
        '\u200B'
        '\r'
        '\n'
    |]
    let nonBreakingSpace = '\u00a0'

    let isCollapsible ch =
        Array.contains ch collapsibleSpace
    let isNonBreaking ch = ch = nonBreakingSpace

module Breaking =
    let breakTagNames = [|
        "blockquote"
        "body"
        "br"
        "center"
        "dd"
        "dir"
        "div"
        "dl"
        "dt"
        "form"
        "h1"
        "h2"
        "h3"
        "h4"
        "h5"
        "h6"
        "head"
        "hr"
        "html"
        "isindex"
        "li"
        "menu"
        "noframes"
        "ol"
        "p"
        "pre"
        "td"
        "th"
        "title"
        "ul"
    |]

    let isBreakingElement (label: string) =
        Array.contains (label.ToLower()) breakTagNames
