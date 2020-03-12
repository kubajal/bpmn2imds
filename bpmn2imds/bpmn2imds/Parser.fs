#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

namespace bpmn2imds

open BPMN
open System.Drawing

type BPMNElement =
    | Process of id: string
    | ExclusiveGateway of id : string * parentId: string
    | ParallelGateway of id : string * parentId: string
    | Task of id : string * parentId: string
    | StartEvent of id : string * parentId: string
    | EndEvent of id : string * parentId: string
    | SequenceFlow of source : string * target: string * id: string * parentId: string
    | MessageFlow of source : string * target: string * id: string * parentId: string
    | BoundaryFlow of source : string * target: string * id: string * parentId: string
    
type Point = Point of x: int * y: int

type BPMNShape = 
    | Diagram of elementRef: string
    | Shape of elementRef: string * m: Point
    | Edge of elementRef: string * s: Point * e: Point
    
module bpmn2imds =
    let Middle  = Point
    let Start   = Point
    let End     = Point
    
    let parse (model: BPMN.Model) = 
        // let model = BPMN.Model.Read("diagram.bpmn")
        
        let planes = 
            model.Diagrams 
            |> Seq.collect(fun e -> e.Planes)
            
        let lastN N (xs: string) = xs.[(xs.Length - N)..]
        let isNull x = match x with null -> true | _ -> false
        let isNotNull x = isNull x |> not
        
        // we are interested not in the ID of the edge (which is of form "<...>_di"
        // but in the <...> part which is saved in e.ElementRef which is the id of the BPMN element
        // that is represented by drawing of the edge
        let edges = 
            planes 
            |> Seq.collect(fun e -> e.Edges)
            |> Seq.map(fun e -> 
                if isNotNull e.ElementRef && isNotNull e.Points && e.Points.Count > 2
                then (e.ElementRef, Seq.head e.Points, Seq.last e.Points) |> Some
                else None)
            |> Seq.map(fun e -> 
                e |> Option.map(fun (elementRef, s, e) -> Edge (elementRef, Start (s.X, s.Y), End (s.X, s.Y))))
        // edges |> printSeq
        
        let shapes = 
            planes 
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
        // shapes |> printSeq

        let elements = model.Elements |> Seq.map(fun e -> 
            match e.TypeName with
            | "process" -> Process (e.ID) |> Some
            | "exclusiveGateway" -> ExclusiveGateway (e.ID, e.ParentID) |> Some
            | "parallelGateway" -> ParallelGateway (e.ID, e.ParentID) |> Some
            | "task" -> Task (e.ID, e.ParentID) |> Some
            | "startEvent" -> StartEvent (e.ID, e.ParentID) |> Some
            | "endEvent" -> EndEvent (e.ID, e.ParentID) |> Some
            | "sequenceFlow" -> SequenceFlow (e.Attributes.["sourceRef"], e.Attributes.["targetRef"], e.ID, e.ParentID) |> Some
            | "messageFlow" -> MessageFlow (e.Attributes.["sourceRef"], e.Attributes.["targetRef"], e.ID, e.ParentID) |> Some
            | _ -> None)
        let flows = model.Elements |> Seq.map(fun e -> 
            match e.TypeName with
            | "sequenceFlow" -> SequenceFlow (e.Attributes.["sourceRef"], e.Attributes.["targetRef"], e.ID, e.ParentID) |> Some
            | "messageFlow" -> MessageFlow (e.Attributes.["sourceRef"], e.Attributes.["targetRef"], e.ID, e.ParentID) |> Some
            | "boundaryEvent" -> BoundaryFlow (e.Attributes.["attachedToRef"],  e.ID, (lastN 7 e.Attributes.["attachedToRef"]) + (lastN <| 7 <| e.ID), e.ParentID) |> Some
            | _ -> None)
        flows |> printSeq
            
        let printSeq seq1 = Seq.iter (printf "%A\n") seq1; printfn ""
        elements |> printSeq

(*    elif (e.TypeName == "task") then Task e.ID
    elif (e.TypeName == "startEvent") then StartEvent e.ID
    elif (e.TypeName == "endEvent") then EndEvent e.ID
    elif (e.TypeName == "sequenceFlow") then SequenceFlow (e.Attributes["sourceRef"], e.Attributes["targetRef"], e.ID)
    elif (e.TypeName == "sequenceFlow") then Messageflow (e.Attributes["sourceRef"], e.Attributes["targetRef"], e.ID))
*)        
// let essentialGraph = bpmn2imds.Graphs.FirstLevel.EssentialGraph(bpmn)