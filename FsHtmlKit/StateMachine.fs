module FsHtmlKit.StateMachine

open FSharp.Data
open FSharp.Data.HtmlActivePatterns

module StringExt = FsCombinators.StringExtensions

open BufferManipulation
open HtmlRules.Space
open HtmlRules.Breaking
open HtmlRules.Headings

type CollapseState =
    | Init
    | InSpace
    | InWord

type MachineState = string * CollapseState
type LineBreakAction = MachineState -> MachineState
type CollapseAction = string -> MachineState -> MachineState

let visitNode lba ca transform inputState (node: HtmlNode) : MachineState =
    match node with
    | HtmlElement (name, _, _) when (isHeading name || isBreakingElement name) -> lba
    | HtmlText textValue when textValue.Equals(newline) -> lba
    | _ -> ca (transform node)
    <| inputState

let carriageReturn: LineBreakAction =
    fun (buffer: string, _) ->
        match String.length buffer with
        | n when
            n > 0
            && (not (buffer |> StringExt.endsWith newline))
            ->
            (appendNewline buffer, Init)
        | _ -> (buffer, Init)

let collapse: CollapseAction =
    fun text (buffer, state) ->
        let transitionOnCharacter (buffer', state') character =
            if (isCollapsible character) then
                match state' with
                | Init -> (buffer', Init)
                | _ -> (buffer', InSpace)
            else
                let appendCharTo =
                    StringExt.appendString (sprintf "%c" character)

                match state' with
                | InSpace -> (appendSpace buffer' |> appendCharTo, InWord)
                | _ -> (buffer' |> appendCharTo, InWord)

        match String.length text with
        | 0 -> (buffer, state)
        | _ ->
            text
            |> Seq.fold transitionOnCharacter (buffer, state)

let initialState: MachineState = ("", Init)

let runMachine: (HtmlNode -> string) -> HtmlNode seq -> MachineState =
    fun transform -> Seq.fold (visitNode carriageReturn collapse transform) initialState
