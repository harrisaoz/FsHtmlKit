module FsHtmlKit.StateMachine

open FsHtmlKit.NodeTypes

module StringExt = FsCombinators.StringExtensions

open BufferManipulation
open HtmlRules.Space

module N = NodeTypes

type CollapseState =
    | Init
    | InSpace
    | InWord

type MachineState = string * CollapseState
type LineBreakAction = MachineState -> MachineState
type CollapseAction = string -> MachineState -> MachineState

///
/// <param name="asVisitedNode">Transform the implementation-specific node to a
/// matchable expression of type NodeTypes.VisitedNode</param>
/// <param name="lba">line-break action</param>
/// <param name="ca">collapse action</param>
/// <param name="transform">transform the node according to the desired output
/// format</param>
/// <param name="inputState">The state of the fsm when visitNode is called
/// </param>
/// <param name="node">The node to visit</param>
///
let visitNode lba ca asVisitedNode transform inputState node : MachineState =
    let visitedNodeType = asVisitedNode node
    match visitedNodeType with
    | BreakingElement -> lba
    | HeadingElement -> lba
    | NewLineTextNode -> lba
    | OtherNode -> ca (transform node)
    <| inputState

let carriageReturn: LineBreakAction =
    fun (buffer: string, _) ->
        match String.length buffer with
        | n when n > 0 && (buffer |> (not << StringExt.endsWith newline)) ->
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
        | _ -> text |> Seq.fold transitionOnCharacter (buffer, state)

let initialState: MachineState = ("", Init)

let runMachine: LineBreakAction
    -> CollapseAction
    -> ('Node -> N.VisitedNode)
    -> ('Node -> string)
    -> 'Node seq
    -> MachineState =
    fun lba ca asVisitedNode transform ->
        Seq.fold (visitNode lba ca asVisitedNode transform) initialState

module Std =
    let inline runMachine asVisitedNode transform =
        runMachine carriageReturn collapse asVisitedNode transform

module Fsd =
    let inline runMachine transform =
        Std.runMachine FsdAdapter.asVisitedNode transform

module Hap =
    let inline runMachine transform =
        Std.runMachine HapAdapter.asVisitedNode transform
