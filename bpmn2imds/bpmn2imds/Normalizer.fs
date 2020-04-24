namespace bpmn2imds

open Utils

module Normalizer =

    let normalize (
        element: BPMNElement,
        incSeq: seq<BPMNFlow>,
        outSeq: seq<BPMNFlow>,
        incMes: seq<BPMNFlow>,
        outMes: seq<BPMNFlow>) =
        1