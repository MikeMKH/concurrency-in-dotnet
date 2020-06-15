module Program

open System.Collections.Generic
open System.Collections.Concurrent
open System.Linq
open FSharp.Collections.ParallelSeq

module Memoization =
  let memoize (f: 'a -> 'b) =
    let m = ConcurrentDictionary<'a,'b>()
    fun x ->   m.GetOrAdd(x, f)

module ConsurrentSpeculation =
  let fuzzyMatch (numbers: int list) =
  let numbersSet = new HashSet<int>(numbers)
  fun number ->
      numbersSet
        |> PSeq.sortBy(fun n -> abs (number - n))
        |> Seq.head