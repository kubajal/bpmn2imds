namespace bpmn2imds

open NUnit.Framework
open bpmn2imds

module utils_unit_tests =

    [<TestFixture>]
    type bpmn2imds_utils_tests () =
    
        [<Test>]
        member this.LastNShouldWorkOnEmptyString () =
            Assert.IsTrue((parser.lastN 1 "") = "")
    
        [<Test>]
        member this.LastNShouldWorkOnSingleChar () =
            Assert.IsTrue((parser.lastN <| 1 <| "a") = "a")
    
        [<Test>]
        member this.LastNShouldWorkWhenNIsBiggerThanLength() =
            Assert.IsTrue((parser.lastN <| 5 <| "abc") = "abc")
    
        [<Test>]
        member this.LastNShouldWorkWhenNIsSmallerThanLength() =
            Assert.IsTrue((parser.lastN <| 2 <| "abc") = "bc")

        [<Test>]
        member this.LastNShouldWorkWhenNIsEqualToLength() =
            Assert.IsTrue((parser.lastN <| 3 <| "abc") = "abc")
    
        [<Test>]
        member this.IsNullShouldWorkOnNull() =
            Assert.IsTrue(parser.isNull null = true)

        [<Test>]
        member this.IsNullShouldWorkOnNotNull() =
            Assert.IsTrue(parser.isNull "asd" = false)

        [<Test>]
        member this.IsNotNullShouldWorkOnNull() =
            Assert.IsTrue(parser.isNotNull null = false)

        [<Test>]
        member this.IsNotNullShouldWorkOnNotNull() =
            Assert.IsTrue(parser.isNotNull "asd" = true)