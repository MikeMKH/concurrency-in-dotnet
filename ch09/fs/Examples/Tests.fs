module Tests

open System
open AsyncExtension
open Xunit

[<Fact>]
let ``AsyncExtension map is composable`` () =
  let foo = async {
    return 1
  }
  
  let result =
    foo
    |> map (fun x -> x + 1)
    |> map (fun x -> x * 3)
    |> Async.RunSynchronously
  
  Assert.Equal((1 + 1) * 3, result)
