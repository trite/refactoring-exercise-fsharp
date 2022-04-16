module UnknownLibTests

open NUnit.Framework
open FsUnit

open UnknownLib

// These tests are using results from the original javascript doThingsAndStuff
// Effectively translated from the ReasonML version of the code attempt

[<Test>]
let ``single space string`` () =
    doThingsAndStuff ["foo"; " "; "bar"]
    |> should equal  [" "]

[<Test>]
let ``removes strings without spaces and partially orders`` () =
    doThingsAndStuff ["   "; "foo"; " "; "bar"; "     "]
    |> should equal  ["     "; " "; "   "]

[<Test>]
let ``finishes ordering spaces`` () =
    doThingsAndStuff ["     "; " "; "   "]
    |> should equal  ["     "; " "; "   "]

[<Test>]
let ``no more changes to order`` () =
    doThingsAndStuff ["     "; " "; "   "]
    |> should equal  ["     "; " "; "   "]

[<Test>]
let ``removes '1' and begins ordering the rest`` () =
    doThingsAndStuff ["1"; " 2"; "3 "; " 4 "; "  5"; "6  "; "  7  "; "8 8 8"; " 9 9 "]
    |> should equal  [" 2"; " 4 "; " 9 9 "; "8 8 8"; "  7  "; "6  "; "  5"; "3 "]

[<Test>]
let ``continues ordering numbers`` () =
    doThingsAndStuff [" 2"; " 4 "; " 9 9 "; "8 8 8"; "  7  "; "6  "; "  5"; "3 "]
    |> should equal  [" 2"; " 4 "; " 9 9 "; "8 8 8"; "  7  "; "  5"; "3 "; "6  "]

[<Test>]
let ``finishes ordering numbers`` () =
    doThingsAndStuff [" 2"; " 4 "; " 9 9 "; "8 8 8"; "  7  "; "  5"; "3 "; "6  "]
    |> should equal  [" 2"; " 4 "; " 9 9 "; "8 8 8"; "  7  "; "  5"; "6  "; "3 "]

[<Test>]
let ``no more changes to number order`` () =
    doThingsAndStuff [" 2"; " 4 "; " 9 9 "; "8 8 8"; "  7  "; "  5"; "6  "; "3 "]
    |> should equal  [" 2"; " 4 "; " 9 9 "; "8 8 8"; "  7  "; "  5"; "6  "; "3 "]

[<Test>]
let ``simple pairing that should remain unmodified`` () =
    doThingsAndStuff [" 1"; "2 "]
    |> should equal  [" 1"; "2 "]

[<Test>]
let ``simple pairing that should reverse positions`` () =
    doThingsAndStuff ["2 "; " 1"]
    |> should equal  [" 1"; "2 "]

[<Test>]
let ``remove items and fully sort`` () =
    doThingsAndStuff ["a"; " b"; "c "; "d"; "eeeee"; "f  f"; "gg"; "  "]
    |> should equal  [" b"; "f  f"; "  "; "c "]

[<Test>]
let ``no more sorting to be done`` () =
    doThingsAndStuff [" b"; "f  f"; "  "; "c "]
    |> should equal  [" b"; "f  f"; "  "; "c "]

[<Test>]
let ``only spaces being reordered`` () =
    doThingsAndStuff [" "; "   "; "     "]
    |> should equal  ["     "; "   "; " "]

[<Test>]
let ``spaces that are already sorted`` () =
    doThingsAndStuff ["     "; "   "; " "]
    |> should equal  ["     "; "   "; " "]

[<Test>]
let ``spaces that partially reorder on first run`` () =
    doThingsAndStuff ["   "; " "; "     "]
    |> should equal  ["     "; " "; "   "]

[<Test>]
let ``spaces that finish reordering on second run`` () =
    doThingsAndStuff ["     "; " "; "   "]
    |> should equal  ["     "; "   "; " "]

