module FsHtmlKit.BufferManipulation

module StringExt = FsCombinators.StringExtensions

let newline = System.Environment.NewLine

let appendSpace = StringExt.appendString " "

let appendNewline = StringExt.appendString newline

let replaceNbspWith replacement (inText: string) =
    inText.Replace(HtmlRules.Space.nonBreakingSpace, replacement)

let replaceNbsp = replaceNbspWith ' '
