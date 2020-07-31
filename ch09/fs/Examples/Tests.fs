module Tests

open System
open AsyncExtension
open Program
open Xunit

[<Fact>]
let ``AsyncExtension map is composable`` () =
  let foo = async { return 1 }
  let result =
    foo
    |> map (fun x -> x + 1)
    |> map (fun x -> x * 3)
    |> Async.RunSynchronously
  
  Assert.Equal((1 + 1) * 3, result)
  
[<Fact>]
let ``AsyncExtension tap can be used for side effects`` () =
  let foo = async { return 8 }
  let result =
    foo
    |> tap (fun x -> printfn "tap %i" x)
    |> map (fun x -> x * 2)
    |> Async.RunSynchronously
    
  Assert.Equal(8 * 2, result)

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
  
[<Fact>]
let ``LoggerBuilder can be nested`` () =
  let outer = MonadicLoggerBuilder "Outer Logger Test"
  let inner = MonadicLoggerBuilder "Inner Logger Test"
  let result = outer {
    let! x = async { return 1 }
    let! y = async { return 1 + 1 }
    let z = inner {
      let a = 1
      let! b = async { return 2 }
      return a + (b |> Async.RunSynchronously)
    }
    return
      (x |> map((+) z) |> Async.RunSynchronously)
      + (y |> Async.RunSynchronously)
  }
  
  Assert.Equal(1 + 2 + 3, result)