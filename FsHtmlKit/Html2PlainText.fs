module FsHtmlKit.Html2PlainText

/// Extract plain text from the provided document text, assuming that the text
/// is formatted as valid HTML, but not necessarily a complete document, and
/// not necessarily all under a single parent element - that is, numerous
/// siblings may be present at the top level of the document text.
///
/// On successful extraction, the plain text is returned in an Option.Some
/// instance.
/// On failure, Option.None is returned.
let tryExtractPlainText
    elements
    parse
    getChildren
    asNodeType
    asVisitedNode
    attrName
    attrValue
    (docText: string)
    =
    docText
    |> Parse.tryParse parse
    |> Option.fold elements Seq.empty
    |> Seq.collect (
        Enumeration.filteredDescendents
            getChildren
            (AsPlainText.elementsAndTextOnly asNodeType)
    )
    |> StateMachine.Std.runMachine
        asVisitedNode
        (AsPlainText.asPlainText asNodeType attrName attrValue)
    |> fst
    |> function
        | "" -> None
        | text -> Some text

/// Use tryExtractPlainText (see documentation for details) to extract plain
/// text from the input HTML document.
/// On success, the extracted plain text is returned.
/// On failure, the original text is returned.
let html2Text
    elements
    parse
    getChildren
    asNodeType
    asVisitedNode
    attrName
    attrValue
    (docText: string)
    =

    docText
    |> tryExtractPlainText
        elements
        parse
        getChildren
        asNodeType
        asVisitedNode
        attrName
        attrValue
    |> function
        | None -> docText
        | Some buffer -> buffer

module UsingAdapter =
    open LibAdapter

    let tryExtractPlainText (adapter: HtmlLibAdapter<'D, 'N, 'A>) =
        tryExtractPlainText
            (documentElements adapter)
            (parse adapter)
            (childNodes adapter)
            (asTransformableNode adapter)
            (asVisitedNode adapter)
            (attributeName adapter)
            (attributeValue adapter)

    let html2Text (adapter: HtmlLibAdapter<'D, 'N, 'A>) docText =
        tryExtractPlainText adapter docText |> Option.defaultValue docText

module Fsd =
    let tryExtractPlainText (docText: string) =
        UsingAdapter.tryExtractPlainText FsdAdapter.fsdLibAdapter docText

    let html2Text (docText: string) =
        UsingAdapter.html2Text FsdAdapter.fsdLibAdapter docText

module Hap =
    let tryExtractPlainText (docText: string) =
        UsingAdapter.tryExtractPlainText HapAdapter.hapLibAdapter docText

    let html2Text (docText: string) =
        UsingAdapter.html2Text HapAdapter.hapLibAdapter docText

module Std =
    let tryExtractPlainText (docText: string) = Hap.tryExtractPlainText docText

    let html2Text (docText: string) = Hap.html2Text docText
