module UnknownLib

//let hello (name: string): string =
//    $"Hello {name}"
 
//let doThingsAndStuff (lst: string list): string list =
//    lst
//    |> List.filter (fun s -> s.Contains(' '))
//    |> List.sort
//    |> List.rev

type Examining =
    {
        remaining: char list
        item: char
    }

type Comparing =
    {
        previousStrings: string list
        remainingStrings: string list
        examining: Examining
        spaceFound: bool
    }

type CompareState =
    {
        pullFrom: string list
        pushTo: string list
        examining: Examining
        comparing: Comparing
    }

let rec mainLoop (pullFrom: string list) (pushTo: string list) : string list =
    match pullFrom with
    | [] ->
        pushTo |> List.rev
    | x::rest ->
        examine rest pushTo x

and examine (pullFrom: string list) (pushTo: string list) (toExamine: string) : string list =
    if toExamine.Contains(' ') then
        prepare pullFrom pushTo toExamine
    else
        // no space, drop it
        mainLoop pullFrom pushTo

and prepare pullFrom pushTo toExamine : string list =
    //let examine::remainingChars =
    //    toExamine.ToCharArray()
    //    |> Array.toList

    //let getCompareExam (compareItem : string) : Examining =
    let (| Examine | Next |) (compareItem : string) =
        match compareItem.ToCharArray() |> Array.toList with
        | compareChar::compareRemainingChars ->
            Examine({
                remaining = compareRemainingChars
                item = compareChar
            })
        | [] ->
            Next

    let (| Compare | Done |) (pushTo : string list) =
        let rec getNextCompare pushTo =
            match pushTo with
            | compareItem::compareRemainingStrings ->
                match compareItem with
                | Examine(ci) ->
                    Compare({
                        previousStrings = []
                        remainingStrings = compareRemainingStrings
                        examining = ci
                        spaceFound = false
                    })
                | Next ->
                    getNextCompare compareRemainingStrings
            | [] ->
                Done

        getNextCompare pushTo

    match toExamine.ToCharArray() |> Array.toList with
    | c::cs ->
        match pushTo with
        | Compare(comparing) ->
            {
                pullFrom = pullFrom
                pushTo = pushTo
                examining = {
                    remaining = cs
                    item = c
                }
                comparing = comparing
            }
            |> compare
        | Done ->
            let newPushTo =
                if pushTo |> List.contains toExamine then
                    pushTo
                else
                    toExamine::pushTo

            mainLoop pullFrom newPushTo
    | [] ->
        mainLoop pullFrom (toExamine::pushTo)

and compare (state: CompareState) : string list =

    

let doThingsAndStuff (lst: string list): string list =
    mainLoop lst []
