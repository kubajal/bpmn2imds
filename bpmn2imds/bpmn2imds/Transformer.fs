namespace bpmn2imds

type ValidationError =
    | EndEventShouldNotHaveOutgoingFlows of id: string
    | StartEventShouldNotHaveIncomingFlows of id: string

type Warning =
    | EndEventDoesNotHaveIncomingFlows of id: string

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

type Flow = Flow of flowType: FlowType * source: Node * target: Node * id: string * left: Point * right: Point

module Transformer =

    let x y = y

    //let transform (element: BPMNElement) (incoming: seq<BPMNFlow>) (outgoing: seq<BPMNFlow>) =
    //    match element with
    //    | BPMNElement (ExclusiveGateway, id, parent, middle) -> Ok ([XOR (id, middle, id)], Seq.append incoming outgoing, [])
    //    | BPMNElement (EndEvent, id, parent, middle) -> 
    //        match (Seq.length incoming, Seq.length outgoing) with
    //        | (0, 0) -> Ok ([], Seq.empty, [EndEventDoesNotHaveIncomingFlows id])
    //        | (1, 0) -> Ok ([End (id, middle, id)], incoming, [])
    //        | (_, 0) -> 
    //            let source = XOR (id + "_xor", middle, id)
    //            let target = End (id + "_end", middle, id)
    //            let link = Link (id + "_xor", id + "_end", id + "_link", middle, middle)
    //            let incomingToXor = incoming |> Seq.map(function
    //                | SequenceFlow (s, t, id, left, right) -> Sequence (s, source, id + "_xor", left, right)
    //                | MessageFlow (s, t, id, left, right) -> Message (s, source, id + "_xor", left, right)
    //                | BoundaryFlow (s, t, id, left, right) -> Boundary (s, source, id + "_xor", left, right))
    //            Ok ([source; target], Seq.append incoming outgoing, [])
    //        | (_, _) -> Error (EndEventShouldNotHaveOutgoingFlows id)
    //    | BPMNElement (StartEvent, id, parent, middle) -> 
    //        match (Seq.length incoming, Seq.length outgoing) with
    //        | (0, 0) -> Ok []
    //        | (0, 1) -> Ok [Start (id, middle)]
    //        | (0, _) -> Ok [Start (id, middle); AND (id, middle)]
    //        | (_, _) -> Error (StartEventShouldNotHaveIncomingFlows id)
    //    | BPMNElement (Activity, id, parent, middle) ->
    //        match (Seq.length incoming, Seq.length outgoing) with
    //        | (0, 0) -> Ok []
    //        | (0, 1) -> Ok [Start (id, middle)]
    //        | (0, _) -> Ok [Start (id, middle); AND (id, middle)]
    //        | (1, 0) -> Ok [End (id, middle)]
    //        | (_, 0) -> Ok [XOR (id, middle); End (id, middle)]
    //        | (1, 1) -> Ok [XOR (id, middle)]
    //        | (_, 1) -> Ok [XOR (id, middle)]
    //        | (1, _) -> Ok [AND (id, middle)]
    //        | (_, _) -> Ok [XOR (id, middle); AND (id, middle)]
    
    //let transformBPMNElements (flows: seq<BPMNFlow>) = 
    //    flows |> Seq.map(fun e -> 
    //        match e with
    //        | )