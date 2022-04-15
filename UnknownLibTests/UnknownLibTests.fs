module UnknownLibTests

open NUnit.Framework
open FsUnit

open UnknownLib

//let equalTo left right =
//    Assert.AreEqual(left, right)

[<Test>]
let ``sanity check test`` () =
    hello "foo" |> should equal "Hello foo"


//let doThingsThenCompareTo original expected =
//    doThingsAndStuff original
//    |> equalTo expected

[<Test>]
let ``single space string`` () =
    doThingsAndStuff ["foo"; " "; "bar"]
    |> should equal [" "]

    //doThingsAndStuff ["foo"; " "; "bar"]
    //|> equalTo [" "]
