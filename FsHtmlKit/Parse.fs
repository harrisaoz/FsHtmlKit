module FsHtmlKit.Parse

let tryParse parse (documentText: string) =
    try
        Some <| parse documentText
    with
    | :? System.OutOfMemoryException -> reraise ()
    | _ -> None
