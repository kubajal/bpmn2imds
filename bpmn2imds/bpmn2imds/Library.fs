namespace bpmn2imds_v2

open bpmn2imds

module Say =
    bpmn2imds.Setup.Dependencies.BindForDedan()
    let bpmn = BPMN.Model.Read("test.bpmn")
    let essentialGraph = bpmn2imds.Graphs.FirstLevel.EssentialGraph(bpmn)