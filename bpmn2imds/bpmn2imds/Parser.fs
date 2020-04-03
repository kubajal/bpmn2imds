#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

namespace bpmn2imds

open BPMN
type Point = Point of x: int * y: int

type BPMNElement =
    | Process of id: string
    | ExclusiveGateway of id : string * parentId: string * middle: Point
    | ParallelGateway of id : string * parentId: string * middle: Point
    | Task of id : string * parentId: string * middle: Point
    | StartEvent of id : string * parentId: string * middle: Point
    | EndEvent of id : string * parentId: string * middle: Point
    | IntermediateEvent of id : string * parentId: string * middle: Point
    | BoundaryEvent of id : string * parentId: string * middle: Point
    | GenericNode of id : string * nodeType: string * parentId: string * middle: Point

type BPMNFlow = 
    | SequenceFlow of source : BPMNElement * target: BPMNElement * id: string * left: Point * right: Point
    | MessageFlow of source : BPMNElement * target: BPMNElement * id: string * left: Point * right: Point
    | BoundaryFlow of source : BPMNElement * target: BPMNElement * id: string * left: Point * right: Point
    
type Shape = Shape of elementRef: string * m: Point
type Edge = Edge of elementRef: string * s: Point * e: Point
    
module parser =
    let Middle  = Point
    let Start   = Point
    let End     = Point
    
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
        e |> Option.map(f))
    let ($) left right = left right

    // we are interested not in the ID of the edge (which is of form "<...>_di"
    // but in the <...> part which is saved in e.ElementRef which is the id of the BPMN element
    // that is represented by drawing of the edge
    let getEdges (model: BPMN.Model) = 
        model.Diagrams 
        |> Seq.collect(fun e -> e.Planes)
        |> Seq.collect(fun e -> e.Edges)
        |> Seq.map(fun e -> 
            if isNotNull e.ElementRef && isNotNull e.Points && e.Points.Count > 1
            then (e.ElementRef, Seq.head e.Points, Seq.last e.Points) |> Some
            else None)
        |> Seq.map(fun e -> 
            e |> Option.map(fun (elementRef, s, e) -> Edge (elementRef, Start (s.X, s.Y), End (s.X, s.Y))))
    
    let getShapes (model: BPMN.Model) = 
        model.Diagrams 
        |> Seq.collect(fun e -> e.Planes)
        |> Seq.collect(fun e -> e.Shapes)
        |> Seq.map(fun e -> 
            if isNotNull e.ElementRef && isNotNull e.Bounds && e.Bounds.Count = 1
            then (e.ElementRef, Seq.head e.Bounds) |> Some
            else None)
        |> Seq.map(fun e -> 
            e |> Option.map(fun (elementRef, rectangle) -> 
                elementRef, rectangle.Left + rectangle.Width/2, rectangle.Top + rectangle.Height/2))
        |> Seq.map(fun e -> 
            e |> Option.map(fun (elementRef, X, Y) -> 
                Shape (elementRef, Middle (X, Y))))

    let parse (model: BPMN.Model) = 
        let edges = getEdges model
        let shapes = getShapes model
        let elements = 
            shapes 
                >> (fun (Shape (id, middle)) ->
                            let el = model.ElementByID(id)
                            (el, middle))
                >> (fun (el, middle) ->
                        match el.TypeName with
                            | "exclusiveGateway" -> (el.ID, ExclusiveGateway (el.ID, el.ParentID, middle))
                            | "parallelGateway" -> (el.ID, ParallelGateway (el.ID, el.ParentID, middle))
                            | "task" -> (el.ID, Task (el.ID, el.ParentID, middle))
                            | "startEvent" -> (el.ID, StartEvent (el.ID, el.ParentID, middle))
                            | "endEvent" -> (el.ID, EndEvent (el.ID, el.ParentID, middle))
                            | "boundaryEvent" -> (el.ID, BoundaryEvent (el.ID, el.ParentID, middle))
                            | "intermediateThrowEvent" -> (el.ID, IntermediateEvent (el.ID, el.ParentID, middle))
                            | _ -> (el.ID, GenericNode (el.ID, el.TypeName, el.ParentID, middle)))
                |> Seq.choose id
                |> Map.ofSeq
        let flows = 
            edges
                |> Seq.choose id
                |> Seq.map (fun (Edge (elementRef, left, right)) ->
                        let e = model.ElementByID(elementRef)
                        let source = e.Attributes.["sourceRef"]
                        let target = e.Attributes.["targetRef"]
                        match e.TypeName with
                        | "sequenceFlow" -> SequenceFlow (elements.Item(source), elements.Item(target), e.ID, left, right) |> Some
                        | "messageFlow" -> MessageFlow (elements.Item(source), elements.Item(target), e.ID, left, right) |> Some
                        | "boundaryEvent" -> BoundaryFlow (elements.Item(source), elements.Item(target), (lastN 7 e.Attributes.["attachedToRef"]) + "_" + (lastN 7 e.ID), left, right) |> Some
                        | _ -> None)
                |> Seq.choose id
        (elements, flows)