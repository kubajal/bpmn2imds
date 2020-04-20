#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

namespace bpmn2imds

open BPMN
open Utils

type Point = Point of x: int * y: int

type BPMNElementType = 
    | Process
    | ExclusiveGateway
    | ParallelGateway
    | Activity
    | StartEvent
    | EndEvent
    | IntermediateEvent
    | BoundaryEvent of attachedToRef: string
    | GenericNode of nodeType: string

type BPMNElement = 
| BPMNElement of elementType: BPMNElementType * id: string * parentId: string option * middle: Point

type FlowType =
    | Sequence
    | Message
    | Boundary
    | Link

type BPMNFlow = BPMNFlow of flowType: FlowType * source: BPMNElement * target: BPMNElement * id: string * left: Point * right: Point

type Shape = Shape of elementRef: string * m: Point
type Edge = Edge of elementRef: string * s: Point * e: Point
    
module Processor =
    let Middle  = Point
    let Start   = Point
    let End     = Point

    let parser = new ParserBuilder()

    // we are interested not in the ID of the edge (which is of form "<...>_di"
    // but in the <...> part which is saved in e.ElementRef which is the id of the BPMN element
    // that is represented by drawing of the edge
    let getSeqAndMesEdges (model: BPMN.Model) = 
        parser {
            for e in model.Diagrams 
                |> Seq.collect(fun e -> e.Planes) 
                |> Seq.collect(fun e -> e.Edges) do 
                yield!
                    parser.ValidateEdge e
                        |> Result.map(fun (e, warns) -> (e.ElementRef, Seq.head e.Points, Seq.last e.Points, warns))
                        |> Result.map(fun (elementRef, s, e, warns) -> (Edge (elementRef, Start (s.X, s.Y), End (e.X, e.Y)), warns))
        }
    let getShapes (model: BPMN.Model) = 
        parser {
            for s in model.Diagrams 
                |> Seq.collect(fun e -> e.Planes)
                |> Seq.collect(fun e -> e.Shapes) do 
                yield!
                    parser.ValidateShape s
                        |> Result.map(fun ((elementRef, rectangle), warns) -> 
                            ((elementRef, rectangle.Left + rectangle.Width/2, rectangle.Top + rectangle.Height/2), warns))
                        |> Result.map(fun ((elementRef, X, Y), warns) -> 
                                (Shape (elementRef, Middle (X, Y)), warns))
        }

    let parse (model: BPMN.Model) = 
        let tryElements = parser {
            for e in (getShapes model) do 
                yield!
                    e |> Result.bind(fun(Shape (id, middle), warns) ->
                        let el = model.ElementByID(id)
                        if isNull el then
                            Error (ElementNotFound id)
                        else
                            Ok ((el, middle), warns))
                    |> Result.map(fun ((el, middle), warns) -> 
                        match el.TypeName with
                            | "exclusiveGateway" -> ((el.ID, BPMNElement (ExclusiveGateway, el.ID, Some el.ParentID, middle)), warns)
                            | "parallelGateway" -> ((el.ID, BPMNElement (ParallelGateway, el.ID, Some el.ParentID, middle)), warns)
                            | "task" -> ((el.ID, BPMNElement (Activity, el.ID, Some el.ParentID, middle)), warns)
                            | "startEvent" -> ((el.ID, BPMNElement (StartEvent, el.ID, Some el.ParentID, middle)), warns)
                            | "endEvent" -> ((el.ID, BPMNElement (EndEvent, el.ID, Some el.ParentID, middle)), warns)
                            | "boundaryEvent" -> ((el.ID, BPMNElement (BoundaryEvent el.Attributes.["attachedToRef"], el.ID, Some el.ParentID, middle)), warns)
                            | "intermediateThrowEvent" -> ((el.ID, BPMNElement (IntermediateEvent, el.ID, Some el.ParentID, middle)), warns)
                            | _ -> ((el.ID, BPMNElement (GenericNode el.TypeName, el.ID, Some el.ParentID, middle)), CastingToGenericNode (el.ID, el.TypeName) :: warns))
        }

        let (parsedElements, elementsErrors, elementsWarnings) =
            splitResults tryElements

        let elements = Map.ofSeq parsedElements

        let tryBoundaryFlows = 
            parsedElements 
                |> Seq.map(snd)
                |> Seq.map(function
                    | BPMNElement (BoundaryEvent eventAttachedTo, eventId, eventParent, eventMiddle) ->
                        Some $ match elements.TryFind(eventAttachedTo) with
                        | Some (BPMNElement (Activity, taskId, taskParent, taskMiddle)) -> 
                            Ok ((BPMNFlow (
                                    Boundary,
                                    BPMNElement (Activity, taskId, taskParent, taskMiddle),
                                    BPMNElement (BoundaryEvent eventAttachedTo, eventId, eventParent, eventMiddle),
                                    taskId + "_" + eventId,
                                    taskMiddle,
                                    eventMiddle), []))
                        | Some (BPMNElement (elementType, elementId, elementParent, elementMiddle)) -> Ok((BPMNFlow (
                                    Boundary,
                                    BPMNElement (elementType, elementId, elementParent, elementMiddle),
                                    BPMNElement (BoundaryEvent eventAttachedTo, eventId, eventParent, eventMiddle),
                                    elementId + "_" + eventId,
                                    elementMiddle,
                                    eventMiddle), [AttachedToRefIsNotActivity (eventId, elementId)]))
                        | None -> Error (AttachedToRefNotFound (eventAttachedTo, eventId))
                    | _ -> None)
                |> Seq.choose id
        
        let (boundaryFlows, boundaryFlowsErrors, boundaryFlowsWarns) = 
            splitResults tryBoundaryFlows
            
        let edges = getSeqAndMesEdges model
        let tryFlows = 
            edges |> Seq.map (fun e -> 
                e |> Result.bind (fun (Edge (elementRef, left, right), warns) ->
                    let e = model.ElementByID(elementRef)
                    let source = e.Attributes.["sourceRef"]
                    let target = e.Attributes.["targetRef"]
                    match e.TypeName with
                    | "sequenceFlow" -> Ok $ (BPMNFlow (Sequence, elements.Item(source), elements.Item(target), e.ID, left, right), [])
                    | "messageFlow" -> Ok $ (BPMNFlow (Message, elements.Item(source), elements.Item(target), e.ID, left, right), [])
                    | unknownType -> Error (UknownFlowType unknownType)))

        let (seqOrMesFlows, seqOrMesFlowsError, seqOrMesFlowsWarns) = 
                splitResults tryFlows
        (elements, Seq.append seqOrMesFlows boundaryFlows)