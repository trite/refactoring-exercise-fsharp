module UnknownLib

//let hello (name: string): string =
//    $"Hello {name}"
 
let doThingsAndStuff (lst: string list): string list =
    lst
    |> List.filter (fun s -> s.Contains(' '))
    |> List.sort
    |> List.rev
