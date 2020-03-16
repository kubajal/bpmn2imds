namespace bpmn2imds

type Node =
    | XOR of id: string * middle: Point
    | AND of id: string * middle: Point
    | Start of id: string * middle: Point
    | End of id: string * middle: Point

type Edge =
    | SequenceEdge of source : Node * target: Node * id: string * left: Point * right: Point
    | MessageEdge of source : Node * target: Node * id: string * left: Point * right: Point
    | BoundaryEdge of source : Node * target: Node * id: string * left: Point * right: Point

module Transformer =

    let transform (element: BPMNElement) (incoming: seq<BPMNFlow>) (outgoing: seq<BPMNFlow>) =
        match element with
        | ExclusiveGateway (id, parent, middle) -> Ok [XOR (id, middle)]
        | EndEvent (id, parent, middle) -> 
            match (Seq.length incoming, Seq.length outgoing) with
            | (0, 0) -> Ok []
            | (1, 0) -> Ok [End (id, middle)]
            | (_, 0) -> Ok [XOR (id, middle); End (id, middle)]
            | (_, _) -> Error "End Event should not have outgoing flows!"
        | StartEvent (id, parent, middle) -> 
            match (Seq.length incoming, Seq.length outgoing) with
            | (0, 0) -> Ok []
            | (0, 1) -> Ok [Start (id, middle)]
            | (0, _) -> Ok [Start (id, middle); AND (id, middle)]
            | (_, _) -> Error "Start Event should not have incoming flows!"
        | Task (id, parent, middle) ->
            match (Seq.length incoming, Seq.length outgoing) with
            | (0, 0) -> Ok []
            | (0, 1) -> Ok [Start (id, middle)]
            | (0, _) -> Ok [Start (id, middle); AND (id, middle)]
            | (1, 0) -> Ok [End (id, middle)]
            | (_, 0) -> Ok [XOR (id, middle); End (id, middle)]
            | (1, 1) -> Ok [XOR (id, middle)]
            | (_, 1) -> Ok [XOR (id, middle)]
            | (1, _) -> Ok [AND (id, middle)]
            | (_, _) -> Ok [XOR (id, middle); AND (id, middle)]
    
    let transformBPMNElements (flows: seq<BPMNFlow>) = 
        flows |> Seq.map(fun e -> 
            match e with
            | )