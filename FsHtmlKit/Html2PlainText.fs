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

module Fsd =
    let tryExtractPlainText (docText: string) =
        UsingAdapter.tryExtractPlainText FsdAdapter.fsdLibAdapter docText

module Hap =
    let tryExtractPlainText (docText: string) =
        UsingAdapter.tryExtractPlainText HapAdapter.hapLibAdapter docText

module Std =
    let tryExtractPlainText (docText: string) = Hap.tryExtractPlainText docText
