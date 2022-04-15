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
        original: string
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
    printfn "pullFrom: %A\npushTo: %A" pullFrom pushTo
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
        | [] ->
            Next
        | compareChar::compareRemainingChars ->
            Examine({
                original = compareItem
                remaining = compareRemainingChars
                item = compareChar
            })

    let (| Compare | Done |) (pushTo : string list) =
        let rec getNextCompare pushTo acc =
            match pushTo with
            | [] ->
                Done
            | compareItem::compareRemainingStrings ->
                match compareItem with
                | Next ->
                    getNextCompare compareRemainingStrings (compareItem::acc)
                | Examine(ci) ->
                    Compare({
                        previousStrings = acc
                        remainingStrings = compareRemainingStrings
                        examining = ci
                        spaceFound = false
                    })

        getNextCompare pushTo []

    match toExamine.ToCharArray() |> Array.toList with
    | c::cs ->
        match pushTo with
        | Compare(comparing) ->
            {
                pullFrom = pullFrom
                pushTo = pushTo
                examining = {
                    original = toExamine
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

    let backToMain (state: CompareState) =
        let newPushTo =
            List.append
                state.comparing.remainingStrings
                (state.examining.original::state.comparing.previousStrings)

        mainLoop state.pullFrom newPushTo

    //if (not state.comparing.spaceFound) ||
    //    (state.examining.item < state.comparing.examining.item) then
    if (not state.comparing.spaceFound) && state.comparing.examining.item = ' ' then
        compare {
            state with
                comparing = {
                    state.comparing with
                        spaceFound = true
                }
        }
    else
        if state.examining.item < state.comparing.examining.item then
            backToMain state
        else if state.examining.item = state.comparing.examining.item then
            match state.examining.remaining with
            | e::es ->
                match state.comparing.examining.remaining with
                | c::cs ->
                    compare {
                        state with
                            examining = {
                                state.examining with
                                    remaining = es
                                    item = e
                            }
                            comparing = {
                                state.comparing with
                                    examining = {
                                        state.comparing.examining with
                                            remaining = cs
                                            item = c
                                    }
                            }
                    }
                | [] -> backToMain state
            | [] -> backToMain state

        else
            let newPushTo =
                if state.pushTo |> List.contains state.examining.original then
                    state.pushTo
                else
                    state.examining.original::state.pushTo

            mainLoop state.pullFrom newPushTo

let doThingsAndStuff (lst: string list): string list =
    mainLoop lst []
