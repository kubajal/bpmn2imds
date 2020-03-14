namespace bpmn2imds

#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#r @"C:\Users\user\source\repos\bpmn2imds-v2\bpmn2imds\bpmn2imds\bin\Debug\netcoreapp3.1\bpmn2imds.dll"
#endif
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
        
        let shapes = 
            parser.getShapes model
                |> Seq.filter(fun e -> e.IsSome)
                |> Seq.map(fun e -> e.Value)
        
        let edges = 
            parser.getEdges model
                |> Seq.filter(fun e -> e.IsSome)
                |> Seq.map(fun e -> e.Value)
                
        let printSeq xs = Seq.iter (printfn "%A") xs
    
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
        
        [<Test>]
        member this.ShapesShouldBeParsedCorrectly() =
            shapes |> should contain (Shape ("StartEvent_1rylu38", Point (230, 140)))
            shapes |> should contain (Shape ("Activity_0vx2uea", Point (330,140)))
            shapes |> should contain (Shape ("Event_0ylxgrh", Point (430, 140)))
            shapes |> should contain (Shape ("Event_02rpjh7", Point (230, 300)))
            shapes |> should contain (Shape ("Activity_1en9m0o", Point (320, 300)))
            shapes |> should contain (Shape ("Event_10qvi3n", Point (510, 300)))
            shapes |> should contain (Shape ("Event_11maord", Point (320, 340)))
            shapes |> should contain (Shape ("Event_1onr7og", Point (410, 420)))
            shapes |> should contain (Shape ("Event_19y1pjy", Point (410, 300)))
            shapes |> should contain (Shape ("Participant_1udx6jt", Point (380, 140)))
            shapes |> should contain (Shape ("Participant_065cq1k", Point (390, 355)))
        
        [<Test>]
        member this.EdgesShouldBeParsedCorrectly() =
            edges |> should contain  (Edge ("Flow_108gfjk",Point (248,140),Point (248,140)))
            edges |> should contain  (Edge ("Flow_0levmfs",Point (380,140),Point (380,140)))
            edges |> should contain  (Edge ("Flow_0n1pvzm",Point (248,300),Point (248,300)))
            edges |> should contain  (Edge ("Flow_04zxtx8",Point (370,300),Point (370,300)))
            edges |> should contain  (Edge ("MessageFlow_09q63um",Point (330,180),Point (330,180)))
            edges |> should contain  (Edge ("Flow_1up2kuu",Point (320,358),Point (320,358)))
            edges |> should contain  (Edge ("Flow_0hd9q6u",Point (428,300),Point (428,300)))
            edges |> should contain  (Edge ("MessageFlow_0e15urb",Point (410,282),Point (410,282)))