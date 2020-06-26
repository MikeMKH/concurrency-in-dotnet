module Program
  open System
  open FSharp.Collections.ParallelSeq
  
  let wordPartitoner (lines: string list) =
    lines
      |> PSeq.collect (fun (line: string) -> line.Split(" "))
      |> PSeq.map (fun (word: string) -> word.ToUpper())
      |> PSeq.groupBy id
      |> PSeq.map (fun (word, col) -> (word, col |> Seq.length))
      |> Seq.sortByDescending (fun (_, count) -> count)
      
  let map' (f: 'a -> 'b) col: 'a list =
    Seq.fold (fun m x -> m @ [f x]) [] col
    
  
  let isPrime n =
    if n < 2 then false
    elif n = 2 then true
    else
      let boundary = int(Math.Floor(Math.Sqrt(float(n))))
      [2..boundary]
        |> Seq.exists(fun x -> n % x = 0)
        |> not
    
  let primeSum boundary =
    [|0..boundary|]
      |> Array.Parallel.choose (fun x -> if isPrime x then Some x else None)
      |> Array.sum