namespace bpmn2imds

open Utils

module Normalizer =

    let normalize (
        element: BPMNElement,
        incSeq: seq<BPMNFlow>,
        outSeq: seq<BPMNFlow>,
        incMes: seq<BPMNFlow>,
        outMes: seq<BPMNFlow>) =
        match element with
        | ParallelGateway (id, parent, middle) -> 
            match (Seq.length incSeq, Seq.length outSeq) with
            | (_, _) -> ([XOR (id, parent, middle)], [])
        | BoundaryEvent (id, parent, middle, _) -> 
            match (Seq.length incSeq, Seq.length outSeq) with
            | (_, _) -> ([AND (id, parent, middle)], [])
        | EndEvent (id, parent, middle) -> 
            match (Seq.length incSeq, Seq.length outMes) with
            | (1, 0) -> ([End (id, parent, middle)], [])
            | (x, 0) -> 
                let e = End (id+"_end", parent, middle)
                let XOR = XOR (id+"_xor", parent, middle)
                let link = BPMNFlow (Link, id+"_xor", id+"_end", id, middle, middle)
                ([XOR; e], [link])
        | StartEvent(id, parent, middle) -> 
            match (Seq.length incMes, Seq.length outSeq) with
            | (0, 1) -> ([Start (id, parent, middle)], [])
            | (x, y) -> 
                let start = Start (id+"_start", parent, middle)
                let AND = AND (id+"_and", parent, middle)
                let link = BPMNFlow (Link, id+"_start", id+"_and", id, middle, middle)
                ([start; AND], [link])
        //| ExclusiveGateway (_, _, _) -> 
        //| Activity (_, _, _) -> 