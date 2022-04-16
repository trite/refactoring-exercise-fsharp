module UnknownLib

// If filter/sort was sufficient to solve the task this would do it:
//let doThingsAndStuff (lst: string list): string list =
//    lst
//    |> List.filter (fun s -> s.Contains(' '))
//    |> List.sort
//    |> List.rev

// Used to walk through a string
type Examining =
    {
        original: string
        remaining: char list
        item: char
    }

// Used to walk through an array of strings for figuring
// out where to insert in case an insert is needed
type Comparing =
    {
        previousStrings: string list
        remainingStrings: string list
        examining: Examining
        spaceFound: bool
    }

// Might be better not to group this info, wanted to get things working first
type CompareState =
    {
        pullFrom: string list
        pushTo: string list
        examining: Examining
        comparing: Comparing
    }

// Main loop
// roughly equivalent to: while(x.length)
// "base" in ../img/outline02.png
let rec mainLoop (pullFrom: string list) (pushTo: string list) : string list =
    // attempt to pop the head off the list
    match pullFrom with
    | [] ->
        // no more items to scan
        pushTo |> List.rev
    | x::rest ->
        // examine the head of the list
        examine rest pushTo x

// Examination
// roughly: for (tmep3 = 0; tmep3 < temp2.length; tmep3++)
// "exam" in ../img/outline02.png
and examine (pullFrom: string list) (pushTo: string list) (toExamine: string) : string list =
    if toExamine.Contains(' ') then
        // prepare to compare things
        prepare pullFrom pushTo toExamine
    else
        // no space, drop it and continue on
        mainLoop pullFrom pushTo

// Preparation
// roughly: for (temp4 = 0; temp4 < temp.length; temp4++)
// "prepare" in ../img/outline02.png
and prepare pullFrom pushTo toExamine : string list =
    // Active pattern as an easy way to return finite amounts of
    // possible states without defining a 1-off type declaration
    let (| Examine | Next |) (compareItem : string) =
        match compareItem.ToCharArray() |> Array.toList with
        | [] ->
            // no more characters to scan, signal for next item
            Next
        | compareChar::compareRemainingChars ->
            // good to proceed, return the info wrapped in signal to examine
            Examine({
                original = compareItem
                remaining = compareRemainingChars
                item = compareChar
            })

    let (| Compare | Done |) (pushTo : string list) =
        let rec getNextCompare pushTo acc =
            match pushTo with
            | [] ->
                // no more items to compare against, signal appropriately
                Done
            | compareItem::compareRemainingStrings ->
                match compareItem with
                | Next ->
                    // Next signal received, recurse into next iteration
                    getNextCompare compareRemainingStrings (compareItem::acc)
                | Examine(ci) ->
                    // Examine received, return Comparing record with info needed to proceed and signal
                    Compare({
                        previousStrings = acc
                        remainingStrings = compareRemainingStrings
                        examining = ci
                        spaceFound = false
                    })

        getNextCompare pushTo []

    match toExamine.ToCharArray() |> Array.toList with
    | c::cs ->
        // more characters remaining to examine
        match pushTo with
        | Compare(comparing) ->
            // there are more items to compare, so do it
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
            // nothing else to compare, insert if not present
            let newPushTo =
                if pushTo |> List.contains toExamine then
                    pushTo
                else
                    toExamine::pushTo

            mainLoop pullFrom newPushTo
    | [] ->
        // no more characters to scan
        // this branch can only happen if a space was already found, add to head of list
        mainLoop pullFrom (toExamine::pushTo)

// Comparing 2 items to determine where to insert the main element being examined
// roughly: for (var y = 0; y < temp[temp4].length; y++)
// "compare" in ../img/outline02.png
and compare (state: CompareState) : string list =
    let backToMain (state: CompareState) =
        // before returning to main we need to insert the current item between both halves of `pushTo`
        let newPushTo =
            List.append
                state.comparing.remainingStrings
                (state.examining.original::state.comparing.previousStrings)

        mainLoop state.pullFrom newPushTo

    // TODO: This all got out of hand and has lots of room for improvement
    if (not state.comparing.spaceFound) && state.comparing.examining.item = ' ' then
        // Repeated nearly identically below, huge potential for refactoring
        // if this works as intended (not sure yet)
        match state.examining.remaining with
        | [] ->
            backToMain state
        | e::es ->
            match state.comparing.examining.remaining with
            | [] ->
                let newPushTo = state.examining.original::state.pushTo
                let newToExamine::newPullFrom = state.pullFrom
                prepare newPullFrom newPushTo newToExamine
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
                                spaceFound = true
                                examining = {
                                    state.comparing.examining with
                                        remaining = cs
                                        item = c
                                }
                        }
                }
    else
        if state.examining.item < state.comparing.examining.item then
            backToMain state
        else if state.examining.item = state.comparing.examining.item then

            match state.examining.remaining with
            | [] ->
                match state.pushTo with
                | item::rest -> prepare state.pullFrom rest item
                | [] -> backToMain state
            | e::es ->
                match state.comparing.examining.remaining with
                | [] -> backToMain state
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
        else
            let newPushTo =
                if state.pushTo |> List.contains state.examining.original then
                    state.pushTo
                else
                    state.examining.original::state.pushTo

            mainLoop state.pullFrom newPushTo

// invoke the main loop with an empty accumulator
let doThingsAndStuff (lst: string list): string list =
    mainLoop lst []
