module Program
  open FSharp.Collections.ParallelSeq
  
  let wordPartitoner (lines: string list) =
    lines
      |> PSeq.collect (fun (line: string) -> line.Split(" "))
      |> PSeq.map (fun (word: string) -> word.ToUpper())
      |> PSeq.groupBy id
      |> PSeq.map (fun (word, col) -> (word, col |> Seq.length))
      |> Seq.sortByDescending (fun (_, count) -> count)