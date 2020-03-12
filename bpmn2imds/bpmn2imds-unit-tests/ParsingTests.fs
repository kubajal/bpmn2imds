module bpmn2imds_unit_tests

open NUnit.Framework
open bpmn2imds

[<Test>]
let ElementsShouldBeParsedCorrectly () =
    let model = BPMN.Model.Read("diagram.bpmn")
    let elements = parser.getElements model
    Assert.AreEqual(37, Seq.length elements)