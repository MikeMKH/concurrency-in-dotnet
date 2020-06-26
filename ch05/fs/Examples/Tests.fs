module Tests

open System
open Xunit
open FsCheck
open FsCheck.Xunit
open Program

[<Fact>]
let ``Word Partitioner can split lines`` () =
  let lines = ["hello world"; "good bye sue"; "hello jim"; "one two three"]
  let partitions = wordPartitoner(lines)
  Assert.Equal(9, partitions |> Seq.length)

[<Fact>]
let ``Word Partitioner sorts by number of times word shows up`` () =
  let lines = ["most a"; "b most c"; "d most"; "most e most"]
  let partitions = wordPartitoner(lines)
  Assert.Equal("MOST", partitions |> Seq.head |> fst)

[<Fact>]
let ``Word Partitioner gives count of number of times word shows up`` () =
  let lines = ["five a"; "b c d"; "five e"; "five five five"]
  let partitions = wordPartitoner(lines)
  Assert.Equal(5, partitions |> Seq.head |> snd)
    
[<Fact>]
let ``fold map inc [0; 1] => [1; 2]`` () =
  let inc = (fun x -> x + 1)
  Assert.Equal<Collections.Generic.IEnumerable<int>>(
      [1; 2], [0; 1] |> map' inc)
    
[<Property>]
let ``fold map gives same results as map`` (col: int list, f: int -> int) =
  map' f col = List.map f col

[<Theory>]
[<InlineData(2, 2)>]
[<InlineData(3, 5)>]
[<InlineData(4, 5)>]
[<InlineData(5, 10)>]
[<InlineData(6, 10)>]
[<InlineData(100, 1060)>]
let ``prime sum gives expected total`` (boundary: int, expected: int) =
  Assert.Equal(expected, primeSum boundary)
  
[<Property>]
let ``prime sum gives same value as doing it by hand`` (n: PositiveInt) =
  ((int) n |> primeSum) =  
    (Seq.init ((int) n) (fun x -> x + 1) |> Seq.filter isPrime |> Seq.sum)