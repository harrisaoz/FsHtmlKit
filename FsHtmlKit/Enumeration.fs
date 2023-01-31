module FsHtmlKit.Enumeration

module SeqExtensions = FsCombinators.SeqExtensions

let filteredDescendents enumerateChildNodes filterPredicate =
    SeqExtensions.filteredPreOrder filterPredicate enumerateChildNodes
