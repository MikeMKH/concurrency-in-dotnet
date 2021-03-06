module Tests

open System
open System.Threading
open System.Threading.Tasks
open System.Net
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
  
[<Fact>]
let ``LoggerBuilder can be deeply nested`` () =
  let one = MonadicLoggerBuilder "1 Logger"
  let two = MonadicLoggerBuilder "2 Logger"
  let three = MonadicLoggerBuilder "3 Logger"
  let result = one {
    let! x = async { return 1 }
    let! (y, z) = two {
      let! a = async { return 2 }
      let! b = three {
        return async { return 3 }
      }
      return (a, a |> map((+) (b |> Async.RunSynchronously)))
    }
    return
      x
      |> map((+) (y |> Async.RunSynchronously))
      |> map((+) (z |> Async.RunSynchronously))
  }
  
  Assert.Equal(1 + 2 + 2 + 3, result |> Async.RunSynchronously)

let computation() = async {
  let url = "http://www.thatconference.com/"
  use client = new WebClient()
  let! site = client.AsyncDownloadString(Uri(url))
  return site
}

[<Fact>]
let ``Async.StartWithContinuations example`` () =  
  Async.StartWithContinuations(
    computation(),
    (fun site -> printfn "StartWithContinuations size=%i" site.Length; Assert.True(site.Length > 0)),
    (fun ex -> printfn "Error with StartWithContinuations %s" <| ex.ToString()),
    (fun c -> printfn "Cancel of StartWithContinuations %s" <| c.ToString()))
    
[<Fact>]
let ``Async.Ignore example`` () =
  Async.Ignore(
    computation()
    |> tap (fun site -> printfn "Ignore size=%i" site.Length)
    |> tap (fun site -> Assert.True(site.Length > 0)))

[<Fact>]
let ``Async.Start uses CancellationTokenSource`` () =
  let tokenSource = new CancellationTokenSource()
  let mutable cancel = true
  let compute = async {
    Task.Delay(100)
    cancel <- false
    return 8
  }
  Async.Start(compute |> Async.Ignore, tokenSource.Token)
  tokenSource.Cancel()
  Assert.True(cancel)