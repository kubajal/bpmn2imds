#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

namespace bpmn2imds

type ParsingWarning = 
    | ShapeBoundsCountsGreaterThanOne of elementId: string
    | CastingToGenericNode of elementId: string * elementType: string
    | ShapeBoundsCountNotEqualOne of elementId: string
    | AttachedToRefIsNotActivity of boundaryEventId: string * attachedToRef: string

type ParsingError =
    | ShapeElementRefNull of elementId: string
    | ShapePointsCountLessThanOne of elementId: string
    | ShapePointsNull of elementId: string
    | ElementNotFound of elementId: string
    | UknownFlowType of unknownFlowType: string
    | AttachedToRefNotFound of attachedToRef: string * eventId: string

module Utils =
    let lastN N (xs: string) = xs.[(max (xs.Length - N) 0)..]
    let isNull x = match x with null -> true | _ -> false
    let isNotNull x = isNull x |> not
    let optionMap (ops: seq<'T option>) (f: 'T -> 'S) = 
        ops |> Seq.map(fun e -> e |> Option.map(f))
    let (>>>) opt f = opt |> Option.map(f)
    let (>>>>) opt f = opt |> (fun e ->
        match e with 
        | Some x -> f x
        | _ -> None)
    
    let (>>) xs f = xs |> Seq.map(fun e -> 
        e |> Result.map(f))
    let ($) left right = left right
    let foldResults = fun (succs, erros, warns) e ->
        match e with
        | Error _x -> (succs, _x :: erros, warns)
        | Ok (_x, _y) -> (_x :: succs, erros, _y)
    let splitResults rs =
        Seq.fold foldResults ([], [], []) rs