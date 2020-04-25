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

module NormalizerTests = 

    [<TestFixture>]
    type unit_tests () =
        
        [<Test>]
        member this.StartEventWithOneOutgoingSequenceFlow() =
            let el = StartEvent ("id", Some "parentId", Point (1,2))
            let seq = [BPMNFlow (Sequence, "id", "dummy", "flow", Point (1, 1), Point (1, 1))]
            let (children, flows) = Normalizer.normalize(el, [], seq, [], [])
            Seq.length children |> should equal 1
            Seq.length flows |> should equal 0
            Seq.head children |> should equal (Start ("id", "id", Point (1,2)))

        [<Test>]
        member this.StartEventWithMultipleOutgoingSequenceFlows() =
            let el = StartEvent ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "id", "dummy1", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "id", "dummy2", "flow2", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [], [seq1; seq2], [], [])
            Seq.length children |> should equal 2
            Seq.length flows |> should equal 1
            Seq.head flows |> should equal (BPMNFlow (Link, "id_start", "id_and", "id", Point (1,2), Point (1,2)))
            Seq.head children |> should equal (Start ("id_start", "id", Point (1,2)))
            Seq.last children |> should equal (AND ("id_and", "id", Point (1,2)))
            
        [<Test>]
        member this.StartEventWithIncomingMessageFlowAndMultipleOutgoingSequenceFlows() =
            let el = StartEvent ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "id", "dummy1", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "id", "dummy2", "flow2", Point (1, 1), Point (1, 1))
            let mes = BPMNFlow (Message, "dummy3", "id", "flow3", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [], [seq1; seq2], [mes], [])
            Seq.length children |> should equal 2
            Seq.length flows |> should equal 1
            Seq.head flows |> should equal (BPMNFlow (Link, "id_start", "id_and", "id", Point (1,2), Point (1,2)))
            Seq.head children |> should equal (Start ("id_start", "id", Point (1,2)))
            Seq.last children |> should equal (AND ("id_and", "id", Point (1,2)))
        
        [<Test>]
        member this.EndEventWithOutgoingMessageFlowAndMultipleIncomingSequenceFlows() =
            let el = EndEvent ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "dummy1", "id", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "dummy2", "id", "flow2", Point (1, 1), Point (1, 1))
            let mes = BPMNFlow (Message, "id", "dummy3", "flow3", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq1; seq2], [], [], [mes])
            Seq.length children |> should equal 2
            Seq.length flows |> should equal 1
            Seq.head flows |> should equal (BPMNFlow (Link, "id_xor", "id_and", "id", Point (1,2), Point (1,2)))
            Seq.head children |> should equal (XOR ("id_xor", "id", Point (1,2)))
            Seq.last children |> should equal (AND ("id_and", "id", Point (1,2)))
        
        [<Test>]
        member this.EndEventWithMultipleIncomingSequenceFlows() =
            let el = EndEvent ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "dummy1", "id", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "dummy2", "id", "flow2", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq1; seq2], [], [], [])
            Seq.length children |> should equal 2
            Seq.length flows |> should equal 1
            Seq.head flows |> should equal (BPMNFlow (Link, "id_xor", "id_end", "id", Point (1,2), Point (1,2)))
            Seq.head children |> should equal (XOR ("id_xor", "id", Point (1,2)))
            Seq.last children |> should equal (End ("id_end", "id", Point (1,2)))
        
        [<Test>]
        member this.EndEventWithOneIncomingSequenceFlow() =
            let el = EndEvent ("id", Some "parentId", Point (1,2))
            let seq = BPMNFlow (Sequence, "dummy", "id", "flow", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq], [], [], [])
            Seq.length children |> should equal 1
            Seq.length flows |> should equal 0
            Seq.last children |> should equal (End ("id", "id", Point (1,2)))
            
        [<Test>]
        member this.ParallelGatewayWithOneIncomingSequenceFlow() =
            let el = ParallelGateway ("id", Some "parentId", Point (1,2))
            let seq = BPMNFlow (Sequence, "dummy", "id", "flow", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq], [], [], [])
            Seq.length children |> should equal 1
            Seq.length flows |> should equal 0
            Seq.last children |> should equal (AND ("id", "id", Point (1,2)))
        
        [<Test>]
        member this.ParallelGatewayWithTwoIncomingSequenceFlows() =
            let el = ParallelGateway ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "dummy", "id", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "dummy", "id", "flow2", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq1; seq2], [], [], [])
            Seq.length children |> should equal 1
            Seq.last children |> should equal (AND ("id", "id", Point (1,2)))

        [<Test>]
        member this.ParallelGatewayWithTwoIncomingSequenceFlowsAndOneOutgoingSequenceFlow() =
            let el = ParallelGateway ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "dummy", "id", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "dummy", "id", "flow2", Point (1, 1), Point (1, 1))
            let seq3 = BPMNFlow (Sequence, "id", "dummy", "flow3", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq1; seq2], [seq3], [], [])
            Seq.length children |> should equal 1
            Seq.last children |> should equal (AND ("id", "id", Point (1,2)))
        
        [<Test>]
        member this.ParallelGatewayWithTwoIncomingSequenceFlowsAndTwoOutgoingSequenceFlows() =
            let el = ParallelGateway ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "dummy", "id", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "dummy", "id", "flow2", Point (1, 1), Point (1, 1))
            let seq3 = BPMNFlow (Sequence, "id", "dummy", "flow3", Point (1, 1), Point (1, 1))
            let seq4 = BPMNFlow (Sequence, "id", "dummy", "flow4", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq1; seq2], [seq3; seq4], [], [])
            Seq.length children |> should equal 1
            Seq.last children |> should equal (AND ("id", "id", Point (1,2)))

            
            
        [<Test>]
        member this.ExclusiveGatewayWithOneIncomingSequenceFlow() =
            let el = ExclusiveGateway ("id", Some "parentId", Point (1,2))
            let seq = BPMNFlow (Sequence, "dummy", "id", "flow", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq], [], [], [])
            Seq.length children |> should equal 1
            Seq.length flows |> should equal 0
            Seq.last children |> should equal (XOR ("id", "id", Point (1,2)))
        
        [<Test>]
        member this.ExclusiveGatewayWithTwoIncomingSequenceFlows() =
            let el = ExclusiveGateway ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "dummy", "id", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "dummy", "id", "flow2", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq1; seq2], [], [], [])
            Seq.length children |> should equal 1
            Seq.last children |> should equal (XOR ("id", "id", Point (1,2)))

        [<Test>]
        member this.ExclusiveGatewayWithTwoIncomingSequenceFlowsAndOneOutgoingSequenceFlow() =
            let el = ExclusiveGateway ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "dummy", "id", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "dummy", "id", "flow2", Point (1, 1), Point (1, 1))
            let seq3 = BPMNFlow (Sequence, "id", "dummy", "flow3", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq1; seq2], [seq3], [], [])
            Seq.length children |> should equal 1
            Seq.last children |> should equal (XOR ("id", "id", Point (1,2)))
        
        [<Test>]
        member this.ExclusiveGatewayWithTwoIncomingSequenceFlowsAndTwoOutgoingSequenceFlows() =
            let el = ExclusiveGateway ("id", Some "parentId", Point (1,2))
            let seq1 = BPMNFlow (Sequence, "dummy", "id", "flow1", Point (1, 1), Point (1, 1))
            let seq2 = BPMNFlow (Sequence, "dummy", "id", "flow2", Point (1, 1), Point (1, 1))
            let seq3 = BPMNFlow (Sequence, "id", "dummy", "flow3", Point (1, 1), Point (1, 1))
            let seq4 = BPMNFlow (Sequence, "id", "dummy", "flow4", Point (1, 1), Point (1, 1))
            let (children, flows) = Normalizer.normalize(el, [seq1; seq2], [seq3; seq4], [], [])
            Seq.length children |> should equal 1
            Seq.last children |> should equal (XOR ("id", "id", Point (1,2)))