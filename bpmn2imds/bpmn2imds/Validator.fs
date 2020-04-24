namespace bpmn2imds

open Utils

module Validator =

    let split = fun (succs, erros) e ->
        match e with
        | Error _x -> (succs, _x :: erros)
        | Ok (_x) -> (_x :: succs, erros)
    let splitResults rs =
        List.fold split ([], []) rs

    let checkRule (rule, id, incSeq, outSeq, incMes, outMes) =
        match rule with 
        | MaximumIncomingSequenceFlows n -> 
            if Seq.length incSeq > n then
                Error (id, rule)
            else 
                Ok id
        | MaximumOutgoingSequenceFlows n -> 
            if Seq.length outSeq > n then
                Error (id, rule)
            else 
                Ok id
        | MaximumOutgoingMessageFlows n -> 
            if Seq.length outMes > n then
                Error (id, rule)
            else 
                Ok id
        | MaximumIncomingMessageFlows n -> 
            if Seq.length incMes > n then
                Error (id, rule)
            else 
                Ok id
        | MinimumIncomingMessageFlows n ->
            if Seq.length incMes < n then
                Error (id, rule)
            else 
                Ok id
        | MinimumIncomingSequenceFlows n->
            if Seq.length incSeq < n then
                Error (id, rule)
            else 
                Ok id
        | MinimumOutgoingMessageFlows n->
            if Seq.length outMes < n then
                Error (id, rule)
            else 
                Ok id
        | MinimumOutgoingSequenceFlows n ->
            if Seq.length outSeq < n then
                Error (id, rule)
            else 
                Ok id

    let validate (
        element: BPMNElement,
        incSeq: seq<BPMNFlow>,
        outSeq: seq<BPMNFlow>,
        incMes: seq<BPMNFlow>,
        outMes: seq<BPMNFlow>) =
        let id = getId element

        let (cRules, ncRules) = 
            match element with 
            | ParallelGateway (_, _, _) -> 
                ([MaximumOutgoingMessageFlows 0;
                    MaximumIncomingMessageFlows 0], 
                [MinimumOutgoingSequenceFlows 1; 
                    MinimumIncomingSequenceFlows 1])
            | BoundaryEvent (_, _, _, _) -> 
                ([MaximumIncomingMessageFlows 0;
                    MaximumOutgoingMessageFlows 0], [])
            | EndEvent (_, _, _) -> (
                [MaximumOutgoingSequenceFlows 0;
                    MaximumIncomingMessageFlows 0;
                    MaximumOutgoingMessageFlows 1],
                [MinimumIncomingSequenceFlows 1]);
            | StartEvent (_, _, _) -> 
                ([MaximumIncomingSequenceFlows 0;
                    MaximumIncomingMessageFlows 1;
                    MaximumOutgoingMessageFlows 0],
                [MinimumOutgoingSequenceFlows 1]);
            | ExclusiveGateway (_, _, _) -> 
                ([MaximumIncomingMessageFlows 0;
                    MaximumOutgoingMessageFlows 0], 
                [MinimumOutgoingSequenceFlows 1; 
                    MinimumIncomingSequenceFlows 1]);
            | Activity (_, _, _) -> 
                ([], [MinimumOutgoingSequenceFlows 1; 
                    MinimumIncomingSequenceFlows 1]);
        
        let (_, errors) = splitResults (cRules |> List.map(fun rule ->
            checkRule (rule, id, incSeq, outSeq, incMes, outMes)))
        let (_, warns) = splitResults (ncRules |> List.map(fun rule ->
            checkRule (rule, id, incSeq, outSeq, incMes, outMes)))
        (errors, warns)
