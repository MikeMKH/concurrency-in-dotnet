module Program

open System
open System.Threading.Tasks

type ParallelismHelpers =
    static member MaxDepth =
        int (Math.Log(float Environment.ProcessorCount, 2.0))

let rec quicksort aList =
    match aList with
    | [] -> []
    | x :: xs ->
        let lt, gt =
            List.partition (fun n -> n <= x) xs
        let left  = Task.Run(fun () -> quicksort lt)
        let right = Task.Run(fun () -> quicksort gt)
        left.Result @ (x :: right.Result)

let rec quicksortWithDepth depth aList =
    match aList with
    | [] -> []
    | x :: xs ->
        let lt, gt =
            List.partition (fun n -> n <= x) xs
        if depth < 0 then
          let left  = lt |> quicksortWithDepth depth
          let right = gt |> quicksortWithDepth depth
          left @ (x :: right) 
        else
          let left  = Task.Run(fun () -> lt |> quicksortWithDepth (depth - 1))
          let right = Task.Run(fun () -> gt |> quicksortWithDepth (depth - 1))
          left.Result @ (x :: right.Result)