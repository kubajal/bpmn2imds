
open NUnit.Framework
open bpmn2imds
open FsUnit

module tests = 

    [<TestFixture>]
    type bpmn2imds_unit_tests () =

        let model = BPMN.Model.Read("diagram.bpmn")
        let elements = 
            parser.getElements model
                |> Seq.filter(fun e -> e.IsSome)
                |> Seq.map(fun e -> e.Value)

        let flows = 
            parser.getFlows model
                |> Seq.filter(fun e -> e.IsSome)
                |> Seq.map(fun e -> e.Value)
    
        [<Test>]
        member this.NumberOfElementsShouldBeCorrectCorrectly() =
            Assert.AreEqual(11, Seq.length elements)

        [<Test>]
        member this.StartEventsShouldBeParsedCorrectly() =
            let startEvents = elements |> Seq.filter(fun e -> 
                match e with
                    | StartEvent _ -> true
                    | _ -> false)
            Assert.AreEqual(2, Seq.length startEvents)
            startEvents |> should contain (StartEvent ("StartEvent_1rylu38", "Process_1dsnu7p"))
            startEvents |> should contain (StartEvent ("Event_02rpjh7", "Process_16buli2"))

        [<Test>]
        member this.EndEventsShouldBeParsedCorrectly() =
            let endEvents = elements |> Seq.filter(fun e -> 
                match e with
                    | EndEvent _ -> true
                    | _ -> false)
            Assert.AreEqual(3, Seq.length endEvents)
            endEvents |> should contain (EndEvent ("Event_0ylxgrh", "Process_1dsnu7p"))
            endEvents |> should contain (EndEvent ("Event_10qvi3n", "Process_16buli2"))
            endEvents |> should contain (EndEvent ("Event_1onr7og", "Process_16buli2"))
        
        [<Test>]
        member this.IntermediateEventsShouldBeParsedCorrectly() =
            let intermediateEvents = elements |> Seq.filter(fun e -> 
                match e with
                    | IntermediateEvent _ -> true
                    | _ -> false)
            Assert.AreEqual(2, Seq.length intermediateEvents)
            intermediateEvents |> should contain (IntermediateEvent ("Event_11maord", "Process_16buli2"))
            intermediateEvents |> should contain (IntermediateEvent ("Event_19y1pjy", "Process_16buli2"))

        [<Test>]
        member this.TasksShouldBeParsedCorrectly() =
            let intermediateEvents = elements |> Seq.filter(fun e -> 
                match e with
                    | Task _ -> true
                    | _ -> false)
            Assert.AreEqual(2, Seq.length intermediateEvents)
            intermediateEvents |> should contain (Task ("Activity_0vx2uea", "Process_1dsnu7p"))
            intermediateEvents |> should contain (Task ("Activity_1en9m0o", "Process_16buli2"))

        [<Test>]
        member this.SequenceFlowsShouldBeParsedCorrectly() =
            let sequenceFlows = flows |> Seq.filter(fun e -> 
                match e with
                    | SequenceFlow _ -> true
                    | _ -> false)
            Assert.AreEqual(6, Seq.length sequenceFlows)
            sequenceFlows |> should contain (SequenceFlow ("StartEvent_1rylu38", "Activity_0vx2uea", "Flow_108gfjk", "Process_1dsnu7p"))
            sequenceFlows |> should contain (SequenceFlow ("Activity_0vx2uea", "Event_0ylxgrh", "Flow_0levmfs", "Process_1dsnu7p"))
            sequenceFlows |> should contain (SequenceFlow ("Event_02rpjh7", "Activity_1en9m0o", "Flow_0n1pvzm", "Process_16buli2"))
            sequenceFlows |> should contain (SequenceFlow ("Activity_1en9m0o", "Event_19y1pjy", "Flow_04zxtx8", "Process_16buli2"))
            sequenceFlows |> should contain (SequenceFlow ("Event_11maord", "Event_1onr7og", "Flow_1up2kuu", "Process_16buli2"))
            sequenceFlows |> should contain (SequenceFlow ("Event_19y1pjy", "Event_10qvi3n", "Flow_0hd9q6u", "Process_16buli2"))
        
        [<Test>]
        member this.MessageFlowsShouldBeParsedCorrectly() =
            let messageFlows = flows |> Seq.filter(fun e -> 
                match e with
                    | MessageFlow _ -> true
                    | _ -> false)
            Assert.AreEqual(2, Seq.length messageFlows)
            messageFlows |> should contain (MessageFlow ("Activity_0vx2uea", "Activity_1en9m0o", "MessageFlow_09q63um", "Collaboration_0iwsgg6"))
            messageFlows |> should contain (MessageFlow ("Event_19y1pjy", "Activity_0vx2uea", "MessageFlow_0e15urb", "Collaboration_0iwsgg6"))

        [<Test>]
        member this.BoundaryEventsShouldBeParsedCorrectly() =
            let boundaryFlows = flows |> Seq.filter(fun e -> 
                match e with
                    | BoundaryFlow _ -> true
                    | _ -> false)
            Assert.AreEqual(1, Seq.length boundaryFlows)
            boundaryFlows |> should contain (BoundaryFlow ("Activity_1en9m0o", "Event_11maord", "1en9m0o_11maord", "Process_16buli2"))