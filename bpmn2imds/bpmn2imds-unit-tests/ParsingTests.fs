
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