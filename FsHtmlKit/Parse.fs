module FsHtmlKit.Parse

let tryParse (documentText: string) =
    try
        FSharp.Data.HtmlDocument.Parse(documentText)
        |> Some
    with
        | :? System.OutOfMemoryException ->
            reraise()
        | _ ->
            None
