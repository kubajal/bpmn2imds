#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

open BPMN

let x = ExclusiveGateway "asdf"

type BPMNElement =
    | ExclusiveGateway of id : string
    | ParallelGateway of id : string
    | Task of id : string
    | StartEvent of id : string
    | EndEvent of id : string
    | SequenceFlow of source : string * target: string * id: string
    | MessageFlow of source : string * target: string * id: string
    | GenericNode of id: string * nodeType: string

let bpmn = BPMN.Model.Read("test.bpmn").Elements |> Seq.map(fun e -> 
    match e.TypeName with
    | "exclusiveGateway" -> ExclusiveGateway e.ID
    | "parallelGateway" -> ParallelGateway e.ID
    | "task" -> Task e.ID
    | "startEvent" -> StartEvent e.ID
    | "endEvent" -> EndEvent e.ID
    | "sequenceFlow" -> SequenceFlow (e.Attributes.["sourceRef"], e.Attributes.["targetRef"], e.ID)
    | "messageFlow" -> MessageFlow (e.Attributes.["sourceRef"], e.Attributes.["targetRef"], e.ID)
    | _ -> GenericNode (e.ID, e.TypeName))
    
let printSeq seq1 = Seq.iter (printf "%A\n") seq1; printfn ""
bpmn |> printSeq


(*    elif (e.TypeName == "task") then Task e.ID
    elif (e.TypeName == "startEvent") then StartEvent e.ID
    elif (e.TypeName == "endEvent") then EndEvent e.ID
    elif (e.TypeName == "sequenceFlow") then SequenceFlow (e.Attributes["sourceRef"], e.Attributes["targetRef"], e.ID)
    elif (e.TypeName == "sequenceFlow") then Messageflow (e.Attributes["sourceRef"], e.Attributes["targetRef"], e.ID))
*)        
// let essentialGraph = bpmn2imds.Graphs.FirstLevel.EssentialGraph(bpmn)