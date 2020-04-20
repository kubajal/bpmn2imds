namespace bpmn2imds

#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#r @"C:\Users\user\source\repos\bpmn2imds-v2\bpmn2imds\bpmn2imds\bin\Debug\netcoreapp3.1\bpmn2imds.dll"
#r @"C:\Users\user\source\repos\bpmn2imds-v2\bpmn2imds\bpmn2imds-unit-tests\bin\Debug\netcoreapp3.1\FSharp.Core.dll"
#r @"C:\Users\user\source\repos\bpmn2imds-v2\bpmn2imds\bpmn2imds-unit-tests\bin\Debug\netcoreapp3.1\FSUnit.NUnit.dll"
#r @"C:\Users\user\source\repos\bpmn2imds-v2\bpmn2imds\bpmn2imds-unit-tests\bin\Debug\netcoreapp3.1\nunit.framework.dll"
#r @"C:\Users\user\source\repos\bpmn2imds-v2\bpmn2imds\bpmn2imds-unit-tests\bin\Debug\netcoreapp3.1\NUnit3.TestAdapter.dll"
#endif
open NUnit.Framework
open bpmn2imds
open FsUnit

module tests = 

    [<TestFixture>]
    type bpmn2imds_unit_tests () =

        let model = BPMN.Model.Read("diagram.bpmn")
        let (els, flows) = Processor.parse model
        let elements = els |> Map.toSeq |> Seq.map snd

        //let printSeq xs = Seq.iter (printfn "%A") xs
        //printSeq elements
        
        [<Test>]
        member this.TasksShouldBeParsedCorrectly() =
            elements |> should contain (BPMNElement (Activity, "Activity_0vx2uea",Some "Process_1dsnu7p",Point (330,140)))
            elements |> should contain (BPMNElement (Activity, "Activity_1en9m0o",Some "Process_16buli2",Point (320,300)))
            
        [<Test>]
        member this.StartEventsShouldBeParsedCorrectly() =
            elements |> should contain (BPMNElement (StartEvent, "Event_02rpjh7",Some "Process_16buli2",Point (230,300)))
            elements |> should contain (BPMNElement (StartEvent, "StartEvent_1rylu38",Some "Process_1dsnu7p",Point (230,140)))
            
        [<Test>]
        member this.EndEventsShouldBeParsedCorrectly() =
            elements |> should contain (BPMNElement (EndEvent, "Event_0ylxgrh", Some "Process_1dsnu7p",Point (430,140)))
            elements |> should contain (BPMNElement (EndEvent, "Event_10qvi3n", Some "Process_16buli2",Point (510,300)))
            elements |> should contain (BPMNElement (EndEvent, "Event_1onr7og", Some "Process_16buli2",Point (410,420)))
        
        [<Test>]
        member this.BoundaryEventsShouldBeParsedCorrectly() =
            elements |> should contain (BPMNElement (BoundaryEvent "Activity_1en9m0o", "Event_11maord", Some "Process_16buli2", Point (320,340)))

        [<Test>]
        member this.IntermediateEventsShouldBeParsedCorrectly() =
            elements |> should contain (BPMNElement (IntermediateEvent, "Event_19y1pjy", Some "Process_16buli2",Point (410,300)))

        [<Test>]
        member this.RemainingNodesShouldBeParsedCorrectly() =             
            elements |> should contain (BPMNElement (GenericNode "participant", "Participant_065cq1k", Some "Collaboration_0iwsgg6",Point (390,355)))
            elements |> should contain (BPMNElement (GenericNode "participant", "Participant_1udx6jt", Some "Collaboration_0iwsgg6",Point (380,140)))

        [<Test>]
        member this.NumberOfParsedElementsShouldBeCorrect() =
            Seq.length elements |> should equal 11 // 9 nodes + 2 participants
            
        [<Test>]
        member this.SequenceFlowsShouldBeParsedCorrectly() =
            flows |> should contain (BPMNFlow (Sequence, BPMNElement (StartEvent, "StartEvent_1rylu38", Some "Process_1dsnu7p",Point (230,140)), BPMNElement (Activity, "Activity_0vx2uea", Some "Process_1dsnu7p",Point (330,140)),"Flow_108gfjk", Point (248,140),Point (280,140)))
            flows |> should contain (BPMNFlow (Sequence, BPMNElement (Activity, "Activity_0vx2uea", Some "Process_1dsnu7p",Point (330,140)), BPMNElement (EndEvent, "Event_0ylxgrh", Some "Process_1dsnu7p",Point (430,140)),"Flow_0levmfs", Point (380,140),Point (412,140)))
            flows |> should contain (BPMNFlow (Sequence, BPMNElement (StartEvent, "Event_02rpjh7", Some "Process_16buli2",Point (230,300)), BPMNElement (Activity, "Activity_1en9m0o", Some "Process_16buli2",Point (320,300)),"Flow_0n1pvzm", Point (248,300),Point (270,300)))
            flows |> should contain (BPMNFlow (Sequence, BPMNElement (Activity, "Activity_1en9m0o", Some "Process_16buli2",Point (320,300)), BPMNElement (IntermediateEvent, "Event_19y1pjy", Some "Process_16buli2",Point (410,300)), "Flow_04zxtx8",Point (370,300),Point (392,300)))
            flows |> should contain (BPMNFlow (Sequence, BPMNElement (BoundaryEvent "Activity_1en9m0o", "Event_11maord", Some "Process_16buli2", Point (320,340)), BPMNElement (EndEvent, "Event_1onr7og", Some "Process_16buli2",Point (410,420)),"Flow_1up2kuu", Point (320,358),Point (392,420)))
            flows |> should contain (BPMNFlow (Sequence, BPMNElement (IntermediateEvent, "Event_19y1pjy", Some "Process_16buli2",Point (410,300)), BPMNElement (EndEvent, "Event_10qvi3n", Some "Process_16buli2",Point (510,300)),"Flow_0hd9q6u", Point (428,300),Point (492,300)))
        
        [<Test>]
        member this.MessageFlowsShouldBeParsedCorrectly() =
            flows |> should contain (BPMNFlow (Message, BPMNElement (Activity, "Activity_0vx2uea", Some "Process_1dsnu7p",Point (330,140)), BPMNElement (Activity, "Activity_1en9m0o", Some "Process_16buli2",Point (320,300)), "MessageFlow_09q63um",Point (330,180),Point (330,260)))
            flows |> should contain (BPMNFlow (Message, BPMNElement (IntermediateEvent, "Event_19y1pjy", Some "Process_16buli2",Point (410,300)), BPMNElement (Activity, "Activity_0vx2uea", Some "Process_1dsnu7p",Point (330,140)), "MessageFlow_0e15urb",Point (410,282),Point (350,180)))
            
        [<Test>]
        member this.BoundaryFlowsShouldBeParsedCorrectly() =
            flows |> should contain (BPMNFlow (Boundary, BPMNElement (Activity, "Activity_1en9m0o", Some "Process_16buli2",Point (320,300)), BPMNElement (BoundaryEvent "Activity_1en9m0o", "Event_11maord", Some "Process_16buli2", Point (320,340)), "Activity_1en9m0o_Event_11maord",Point (320,300),Point (320,340)))
                    
        [<Test>]
        member this.NumberOfParsedFlowsShouldBeCorrect() =
            Seq.length flows |> should equal 9 // 6 Sequence Flows + 2 Message Flows + 1 Boundary Flow