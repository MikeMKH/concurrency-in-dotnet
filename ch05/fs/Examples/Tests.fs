module Tests

open Xunit
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
