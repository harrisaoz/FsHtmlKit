module FsHtmlKit.Spec.HtmlTextGen

module SeqExt = FsCombinators.SeqExtensions
module BoolExt = FsCombinators.BooleanExtensions

type HtmlText = HtmlText of string

let allValidChars cs =
    let isValidChar (ch: char) =
        [ System.Char.IsLetterOrDigit
          System.Char.IsPunctuation
          System.Char.IsSeparator ]
        |> SeqExt.existsPredicate ch

    Seq.forall isValidChar cs

let create (s: string) =
    let isOk s =
        String.length s < 1000 && allValidChars s

    match s with
    | null -> None
    | nonNull when isOk nonNull -> nonNull.Trim() |> HtmlText |> Some
    | _ -> None

let apply f (HtmlText t) = f t

let value t = apply id t
