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

and examine (pullFrom: string list) (pushTo: string list) (toExamine: string) =
    if toExamine.Contains(' ') then
        prepare pullFrom pushTo toExamine
    else
        // no space, drop it
        mainLoop pullFrom pushTo

and prepare pullFrom pushTo toExamine =
    let examine::remainingChars =
        toExamine.ToCharArray()
        |> Array.toList

    let compareItem::compareRemainingStrings = pushTo

    let compareChar::compareRemainingChars =
        compareItem.ToCharArray()
        |> Array.toList
        
    compare {
        pullFrom = pullFrom
        pushTo = pushTo
        examining = {
            remaining = remainingChars
            item = examine
        }
        comparing = {
            remainingStrings = compareRemainingStrings
            examining = {
                remaining = compareRemainingChars
                item = compareChar
            }
            spaceFound = false
        }
    }

and compare (state: CompareState) =
    

let doThingsAndStuff (lst: string list): string list =
    mainLoop lst []
