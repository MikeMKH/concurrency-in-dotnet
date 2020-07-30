module Tests

open System
open AsyncExtension
open Program
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

[<Fact>]
let ``LoggerBuilder only logs`` () =
  let logger = MonadicLoggerBuilder "Logger Test"
  let result = logger {
    let x = 6
    let y = 7
    let! z = async { return 8 }
    return x * y + (z |> Async.RunSynchronously)
  }
  
  Assert.Equal(6 * 7 + 8, result)