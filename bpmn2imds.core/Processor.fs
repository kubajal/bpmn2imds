#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

namespace bpmn2imds

open BPMN
open Utils

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
                    validateEdge e
                        |> Result.map(fun (e, warns) -> (e.ElementRef, Seq.head e.Points, Seq.last e.Points, warns))
                        |> Result.map(fun (elementRef, s, e, warns) -> (DiagramEdge (elementRef, Start (s.X, s.Y), End (e.X, e.Y)), warns))
        }
    let getShapes (model: BPMN.Model) = 
        parser {
            for s in model.Diagrams 
                |> Seq.collect(fun e -> e.Planes)
                |> Seq.collect(fun e -> e.Shapes) do 
                yield!
                    validateShape s
                        |> Result.map(fun ((elementRef, rectangle), warns) -> 
                            ((elementRef, rectangle.Left + rectangle.Width/2, rectangle.Top + rectangle.Height/2), warns))
                        |> Result.map(fun ((elementRef, X, Y), warns) -> 
                                (DiagramShape (elementRef, Middle (X, Y)), warns))
        }

    let parse (model: BPMN.Model) = 
        let tryElements = parser {
            for e in (getShapes model) do 
                yield!
                    e |> Result.bind(fun(DiagramShape (id, middle), warns) ->
                        let el = model.ElementByID(id)
                        if isNull el then
                            Error (ElementNotFound id)
                        else
                            Ok ((el, middle), warns))
                    |> Result.bind(fun ((el, middle), warns) -> 
                        match el.TypeName with
                            | "exclusiveGateway" -> Ok ((el.ID, ExclusiveGateway (el.ID, Some el.ParentID, middle)), warns)
                            | "parallelGateway" -> Ok ((el.ID, ParallelGateway (el.ID, Some el.ParentID, middle)), warns)
                            | "task" -> Ok ((el.ID, Activity (el.ID, Some el.ParentID, middle)), warns)
                            | "startEvent" -> Ok ((el.ID, StartEvent (el.ID, Some el.ParentID, middle)), warns)
                            | "endEvent" -> Ok ((el.ID, EndEvent(el.ID, Some el.ParentID, middle)), warns)
                            | "boundaryEvent" -> Ok ((el.ID, BoundaryEvent(el.ID, Some el.ParentID, middle, el.Attributes.["attachedToRef"])), warns)
                            | "intermediateThrowEvent" -> Ok ((el.ID, IntermediateEvent (el.ID, Some el.ParentID, middle)), warns)
                            | "participant" -> Ok ((el.ID, Participant (el.ID, Some el.ParentID, middle)), warns)
                            | _ -> Error (UnknownNodeType el.TypeName))
        }

        let (parsedElements, elementsErrors, elementsWarnings) =
            splitResults tryElements

        let elements = Map.ofSeq parsedElements

        let tryBoundaryFlows = 
            parsedElements 
                |> Seq.map(snd)
                |> Seq.map(function
                    | BoundaryEvent (eventId, eventParent, eventMiddle, eventAttachedTo) ->
                        Some $ match elements.TryFind(eventAttachedTo) with
                        | Some (Activity (taskId, taskParent, taskMiddle)) -> 
                            Ok ((BPMNFlow (
                                    Boundary,
                                    taskId,
                                    eventId,
                                    taskId + "_" + eventId,
                                    taskMiddle,
                                    eventMiddle), []))
                        | Some (_x) -> Ok((BPMNFlow (
                                    Boundary,
                                    getId _x,
                                    eventId,
                                    (getId _x) + "_" + eventId,
                                    (getMiddle _x),
                                    eventMiddle), [AttachedToRefIsNotActivity (eventId, (getId _x))]))
                        | None -> Error (AttachedToRefNotFound (eventAttachedTo, eventId))
                    | _ -> None)
                |> Seq.choose id
        
        let (boundaryFlows, boundaryFlowsErrors, boundaryFlowsWarns) = 
            splitResults tryBoundaryFlows
            
        let edges = getSeqAndMesEdges model
        let tryFlows = 
            edges |> Seq.map (fun e -> 
                e |> Result.bind (fun (DiagramEdge (elementRef, left, right), warns) ->
                    let e = model.ElementByID(elementRef)
                    let source = e.Attributes.["sourceRef"]
                    let target = e.Attributes.["targetRef"]
                    match e.TypeName with
                    | "sequenceFlow" -> Ok $ (BPMNFlow (Sequence, getId (elements.Item(source)), getId (elements.Item(target)), e.ID, left, right), [])
                    | "messageFlow" -> Ok $ (BPMNFlow (Message, getId (elements.Item(source)), getId (elements.Item(target)), e.ID, left, right), [])
                    | unknownType -> Error (UknownFlowType unknownType)))

        let (seqOrMesFlows, seqOrMesFlowsError, seqOrMesFlowsWarns) = 
                splitResults tryFlows
        (elements, Seq.append seqOrMesFlows boundaryFlows)