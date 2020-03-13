module bpmn2imds_utils_unit_tests

open NUnit.Framework
open bpmn2imds

[<SetUp>]
let Setup () =
    ()
    
[<Test>]
let LastNShouldWorkOnEmptyString () =
    Assert.IsTrue((parser.lastN <| 1 <| "") = "")
    
[<Test>]
let LastNShouldWorkOnSingleChar () =
    Assert.IsTrue((parser.lastN <| 1 <| "a") = "a")
    
[<Test>]
let LastNShouldWorkWhenNIsBiggerThanLength() =
    Assert.IsTrue((parser.lastN <| 5 <| "abc") = "abc")
    
[<Test>]
let LastNShouldWorkWhenNIsSmallerThanLength() =
    Assert.IsTrue((parser.lastN <| 2 <| "abc") = "bc")

[<Test>]
let LastNShouldWorkWhenNIsEqualToLength() =
    Assert.IsTrue((parser.lastN <| 3 <| "abc") = "abc")
    
[<Test>]
let IsNullShouldWorkOnNull() =
    Assert.IsTrue(parser.isNull null = true)

[<Test>]
let IsNullShouldWorkOnNotNull() =
    Assert.IsTrue(parser.isNull "asd" = false)


[<Test>]
let IsNotNullShouldWorkOnNull() =
    Assert.IsTrue(parser.isNotNull null = false)

[<Test>]
let IsNotNullShouldWorkOnNotNull() =
    Assert.IsTrue(parser.isNotNull "asd" = true)