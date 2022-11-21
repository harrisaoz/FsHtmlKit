module FsHtmlKit.Html2PlainText

open FSharp.Data

module SM = StateMachine
module PT = AsPlainText
module E = Enumeration

/// Extract plain text from the provided document text, assuming that the text
/// is formatted as valid HTML, but not necessarily a complete document, and
/// not necessarily all under a single parent element - that is, numerous
/// siblings may be present at the top level of the document text.
///
/// On successful extraction, the plain text is returned in an Option.Some
/// instance.
/// On failure, Option.None is returned.
let tryExtractPlainText (docText: string) =
    let elements _ doc : HtmlNode seq =
        HtmlDocumentExtensions.Elements doc

    docText
    |> Parse.tryParse
    |> Option.fold elements Seq.empty
    |> Seq.collect (E.filteredDescendents PT.elementsAndTextOnly)
    |> SM.runMachine PT.asPlainText
    |> fst
    |> function
        | "" -> None
        | text -> Some text

/// Akin to tryExtractPlainText, but only succeeds when given input that
/// contains a body element.
let tryExtractPlainTextOfBody (docText: string) =
    docText
    |> Parse.tryParse
    |> Option.bind HtmlDocumentExtensions.TryGetBody
    |> Option.map (
        E.filteredDescendents PT.elementsAndTextOnly
        >> SM.runMachine PT.asPlainText)
    |> Option.map fst

/// Use tryExtractPlainText (see documentation for details) to extract plain
/// text from the input HTML document.
/// On success, the extracted plain text is returned.
/// On failure, the original text is returned.
let html2Text (docText: string) =
    docText
    |> tryExtractPlainText
    |> function
        | None -> docText
        | Some buffer -> buffer
