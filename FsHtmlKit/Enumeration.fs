module FsHtmlKit.Enumeration

module SeqExtensions = FsCombinators.SeqExtensions

let childNodes node =
    FSharp.Data.HtmlNodeExtensions.Elements node

let filteredDescendents filterPredicate =
    SeqExtensions.filteredPreOrder filterPredicate childNodes
