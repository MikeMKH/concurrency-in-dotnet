module PrimeNumbers

  open System
  open System.Linq
  open System.Collections.Concurrent
  open FSharp.Collections.ParallelSeq
  
  let isPrime n =
    if n < 2 then false
    elif n = 2 then true
    else
      let boundary = int(Math.Floor(Math.Sqrt(float(n))))
      [2..boundary]
        |> Seq.exists(fun x -> n % x = 0)
        |> not
  
  let sequentialSum boundary =
    [0..boundary]
      |> Seq.filter isPrime
      |> Seq.sum 
  
  let parallelSum boundary =
    Seq.init (boundary + 1) id
      |> PSeq.withDegreeOfParallelism(Environment.ProcessorCount)
      |> PSeq.withMergeOptions(ParallelMergeOptions.FullyBuffered)
      |> PSeq.filter (isPrime)
      |> Seq.sum
      
  let parallelLinqSum boundary =
    (Seq.init (boundary + 1) id).AsParallel().Where(isPrime).Sum()