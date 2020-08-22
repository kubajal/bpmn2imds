namespace bpmn2imds

open NUnit.Framework
open Utils

module UtilsTests =

    [<TestFixture>]
    type bpmn2imds_utils_tests () =
    
        [<Test>]
        member this.LastNShouldWorkOnEmptyString () =
            Assert.IsTrue((lastN 1 "") = "")
    
        [<Test>]
        member this.LastNShouldWorkOnSingleChar () =
            Assert.IsTrue((lastN <| 1 <| "a") = "a")
    
        [<Test>]
        member this.LastNShouldWorkWhenNIsBiggerThanLength() =
            Assert.IsTrue((lastN <| 5 <| "abc") = "abc")
    
        [<Test>]
        member this.LastNShouldWorkWhenNIsSmallerThanLength() =
            Assert.IsTrue((lastN <| 2 <| "abc") = "bc")

        [<Test>]
        member this.LastNShouldWorkWhenNIsEqualToLength() =
            Assert.IsTrue((lastN <| 3 <| "abc") = "abc")
    
        [<Test>]
        member this.IsNullShouldWorkOnNull() =
            Assert.IsTrue(isNull null = true)

        [<Test>]
        member this.IsNullShouldWorkOnNotNull() =
            Assert.IsTrue(isNull "asd" = false)

        [<Test>]
        member this.IsNotNullShouldWorkOnNull() =
            Assert.IsTrue(isNotNull null = false)

        [<Test>]
        member this.IsNotNullShouldWorkOnNotNull() =
            Assert.IsTrue(isNotNull "asd" = true)