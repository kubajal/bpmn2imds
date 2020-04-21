#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

namespace bpmn2imds

type Point = Point of x: int * y: int

type ValidationRule =
    | MaximumIncomingSequenceFlows of number: int
    | MaximumOutgoingSequenceFlows of number: int
    | MaximumIncomingMessageFlows of number: int
    | MaximumOutgoingMessageFlows of number: int
    | MinimumIncomingSequenceFlows of number: int
    | MinimumOutgoingSequenceFlows of number: int
    | MinimumIncomingMessageFlows of number: int
    | MinimumOutgoingMessageFlows of number: int

type ParsingWarning = 
    | ShapeBoundsCountsGreaterThanOne of elementId: string
    | CastingToGenericNode of elementId: string * elementType: string
    | ShapeBoundsCountNotEqualOne of elementId: string
    | AttachedToRefIsNotActivity of boundaryEventId: string * attachedToRef: string
    | ValidationWarning of nodeId: string * rule: ValidationRule

type ParsingError =
    | ShapeElementRefNull of elementId: string
    | ShapePointsCountLessThanOne of elementId: string
    | ShapePointsNull of elementId: string
    | ElementNotFound of elementId: string
    | UknownFlowType of unknownFlowType: string
    | AttachedToRefNotFound of attachedToRef: string * eventId: string
    | UnknownNodeType of nodeType: string
    | ValidationError of nodeId: string * rule: ValidationRule

type Node = 
    | XOR of id: string * middle: Point * parent: string
    | AND of id: string * middle: Point * parent: string
    | Start of id: string * middle: Point * parent: string
    | End of id: string * middle: Point * parent: string

type FlowSupplement =
    | Sequence of source : string * target: string * id: string * left: Point * right: Point
    | Message of source : string * target: string * id: string * left: Point * right: Point
    | Boundary of source : string * target: string * id: string * left: Point * right: Point
    | Link of source : string * target: string * id: string * left: Point * right: Point

type BPMNElementType = 
    | Process
    | ExclusiveGateway
    | ParallelGateway
    | Activity
    | StartEvent
    | EndEvent
    | IntermediateEvent
    | BoundaryEvent
    | GenericNode

type BPMNElement = 
    | Process of id: string * parentId: string option * middle: Point 
    | ExclusiveGateway of id: string * parentId: string option * middle: Point 
    | ParallelGateway of id: string * parentId: string option * middle: Point 
    | Activity of id: string * parentId: string option * middle: Point 
    | StartEvent of id: string * parentId: string option * middle: Point 
    | EndEvent of id: string * parentId: string option * middle: Point 
    | IntermediateEvent of id: string * parentId: string option * middle: Point 
    | Participant of id: string * parentId: string option * middle: Point 
    | BoundaryEvent of id: string * parentId: string option * middle: Point * attachedToRef: string

type FlowType =
    | Sequence
    | Message
    | Boundary
    | Link
    
type Flow = Flow of flowType: FlowType * source: Node * target: Node * id: string * left: Point * right: Point
    
type BPMNFlow = BPMNFlow of flowType: FlowType * source: BPMNElement * target: BPMNElement * id: string * left: Point * right: Point

type DiagramShape = DiagramShape of elementRef: string * m: Point
type DiagramEdge = DiagramEdge of elementRef: string * s: Point * e: Point
    
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

    let getId (el: BPMNElement) = 
        match el with 
        | Process (id, parentId, middle) -> id
        | ExclusiveGateway (id, parentId, middle: Point) -> id
        | ParallelGateway (id, parentId, middle: Point) -> id
        | Activity (id, parentId, middle: Point) -> id
        | StartEvent (id, parentId, middle: Point) -> id
        | EndEvent (id, parentId, middle: Point) -> id
        | IntermediateEvent (id, parentId, middle: Point) -> id
        | BoundaryEvent  (id, parentId, middle: Point, eventAttachedTo) -> id

    let getMiddle (el: BPMNElement) = 
        match el with 
        | Process (id, parentId, middle) -> middle
        | ExclusiveGateway (id, parentId, middle: Point) -> middle
        | ParallelGateway (id, parentId, middle: Point) -> middle
        | Activity (id, parentId, middle: Point) -> middle
        | StartEvent (id, parentId, middle: Point) -> middle
        | EndEvent (id, parentId, middle: Point) -> middle
        | IntermediateEvent (id, parentId, middle: Point) -> middle
        | BoundaryEvent  (id, parentId, middle: Point, eventAttachedTo) -> middle
