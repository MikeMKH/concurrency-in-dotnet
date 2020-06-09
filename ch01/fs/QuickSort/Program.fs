module Program

open System
open System.Threading.Tasks

let rec quicksort aList =
    match aList with
    | [] -> []
    | x :: xs ->
        let lt, gt =
            List.partition (fun n -> n <= x) xs
        let left  = Task.Run(fun () -> quicksort lt) // #A
        let right = Task.Run(fun () -> quicksort gt)  // #A
        left.Result @ (x :: right.Result)              // #B