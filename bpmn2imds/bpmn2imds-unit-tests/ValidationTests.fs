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

module ValidationTests = 

    [<TestFixture>]
    type unit_tests () =

        [<Test>]
        member this.EndEventWithoutFlowsShouldBeWarning() =
            let el = EndEvent ("id", Some "parentId", Point (1,2))
            let (errors, warnings) = Validator.validate(el, [], [], [], [])
            Seq.length errors |> should equal 0
            Seq.length warnings |> should equal 1
            let (id, rule) = Seq.head warnings
            id |> should equal "id"
            rule |> should equal (MinimumIncomingSequenceFlows 1)
        
        [<Test>]
        member this.EndEventWithOutgoingSequenceFlowShouldBeError() =
            let el = EndEvent ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let seq = [BPMNFlow (Sequence, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], seq, [], [])
            Seq.length errors |> should equal 1
            errors |> should contain ("id", MaximumOutgoingSequenceFlows 0)
            
        [<Test>]
        member this.EndEventWithIncomingMessageFlowShouldBeError() =
            let el = EndEvent ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], mes, [])
            Seq.length errors |> should equal 1
            errors |> should contain ("id", MaximumIncomingMessageFlows 0)
        
        [<Test>]
        member this.EndEventWithOutgoingMessageFlowsShouldBeOks() =
            let el = EndEvent ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], [], mes)
            Seq.length errors |> should equal 0

        [<Test>]
        member this.StartEventWithoutFlowsShouldBeWarning() =
            let el = StartEvent ("id", Some "parentId", Point (1,2))
            let (errors, warnings) = Validator.validate(el, [], [], [], [])
            Seq.length errors |> should equal 0
            Seq.length warnings |> should equal 1
            let (id, rule) = Seq.head warnings
            id |> should equal "id"
            rule |> should equal (MinimumOutgoingSequenceFlows 1)

        [<Test>]
        member this.StartEventWithIncomingSequenceFlowShouldBeError() =
            let el = StartEvent ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let seq = [BPMNFlow (Sequence, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, seq, [], [], [])
            Seq.length errors |> should equal 1
            Seq.length warnings |> should equal 1
            let (id, rule) = Seq.head errors
            id |> should equal "id"
            rule |> should equal (MaximumIncomingSequenceFlows 0)

        [<Test>]
        member this.StartEventWithIncomingMessageFlowShouldBeOk() =
            let el = StartEvent ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], mes, [])
            Seq.length errors |> should equal 0
            Seq.length warnings |> should equal 1

        [<Test>]
        member this.StartEventWithTwoIncomingMessageFlowsShouldBeError() =
            let el = StartEvent ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [
                BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1));
                BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], mes, [])
            Seq.length errors |> should equal 1
            let (id, rule) = Seq.head errors
            id |> should equal "id"
            rule |> should equal (MaximumIncomingMessageFlows 1)

        
        [<Test>]
        member this.ExclusiveGatewayWithNoIncomingSequenceFlowShouldBeWarning() =
            let el = ExclusiveGateway ("id", Some "parentId", Point (1,2))
            let (errors, warnings) = Validator.validate(el, [], [], [], [])
            Seq.length warnings |> should equal 2
            warnings |> should contain ("id", MinimumOutgoingSequenceFlows 1)
            warnings |> should contain ("id", MinimumIncomingSequenceFlows 1)

        [<Test>]
        member this.ExclusiveGatewayWithIncomingMessageFlowShouldBeError() =
            let el = ExclusiveGateway ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], mes, [])
            Seq.length errors |> should equal 1
            errors |> should contain ("id", MaximumIncomingMessageFlows 0)
        
        [<Test>]
        member this.ExclusiveGatewayWithoutgoingMessageFlowShouldBeError() =
            let el = ExclusiveGateway ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], [], mes)
            Seq.length errors |> should equal 1
            errors |> should contain ("id", MaximumOutgoingMessageFlows 0)
        
        [<Test>]
        member this.ActivityWithoutOutgoingSequenceFlowShouldBeWarn() =
            let el = Activity ("id", Some "parentId", Point (1,2))
            let (errors, warnings) = Validator.validate(el, [], [], [], [])
            Seq.length warnings |> should equal 2
            warnings |> should contain ("id", MinimumOutgoingSequenceFlows 1)
        
        [<Test>]
        member this.ActivityWithFlowShoudBeOk() =
            let el = Activity ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let seq = [BPMNFlow (Sequence, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, seq, seq, mes, mes)
            Seq.length warnings |> should equal 0
            Seq.length errors |> should equal 0

        [<Test>]
        member this.ActivityWithoutIncomingSequenceFlowShouldBeWarn() =
            let el = Activity ("id", Some "parentId", Point (1,2))
            let (errors, warnings) = Validator.validate(el, [], [], [], [])
            Seq.length warnings |> should equal 2
            warnings |> should contain ("id", MinimumIncomingSequenceFlows 1)
        
        [<Test>]
        member this.ParallelGatewayWithoutOutgoingSequenceFlowShouldBeWarn() =
            let el = ParallelGateway ("id", Some "parentId", Point (1,2))
            let (errors, warnings) = Validator.validate(el, [], [], [], [])
            Seq.length warnings |> should equal 2
            warnings |> should contain ("id", MinimumOutgoingSequenceFlows 1)
        
        [<Test>]
        member this.ParallelGatewayWithoutIncomingSequenceFlowShouldBeWarn() =
            let el = ParallelGateway ("id", Some "parentId", Point (1,2))
            let (errors, warnings) = Validator.validate(el, [], [], [], [])
            Seq.length warnings |> should equal 2
            warnings |> should contain ("id", MinimumIncomingSequenceFlows 1)

        [<Test>]
        member this.ParallelGatewayWithOutgoingMessageFlowShouldBeError() =
            let el = ParallelGateway ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], [], mes)
            Seq.length errors |> should equal 1
            errors |> should contain ("id", MaximumOutgoingMessageFlows 0)
        
        [<Test>]
        member this.ParallelGatewayWithoutIncomingMessageFlowShouldBeError() =
            let el = ParallelGateway ("id", Some "parentId", Point (1,2))
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], mes, [])
            Seq.length errors |> should equal 1
            errors |> should contain ("id", MaximumIncomingMessageFlows 0)

        [<Test>]
        member this.BoundaryEventWithoutIncomingMessageFlowShouldBeError() =
            let el = BoundaryEvent ("id", Some "parentId", Point (1,2), "id")
            let dummy = Activity ("dummy", Some "parentId", Point (1,2))
            let mes = [BPMNFlow (Message, el, dummy, "flow", Point (1, 1), Point (1, 1))]
            let (errors, warnings) = Validator.validate(el, [], [], mes, [])
            Seq.length errors |> should equal 1
            errors |> should contain ("id", MaximumIncomingMessageFlows 0)
