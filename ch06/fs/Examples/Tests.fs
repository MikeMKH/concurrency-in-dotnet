module Tests

open System
open System.Threading
open FSharp.Control.Reactive
open Xunit

[<Fact>]
let ``example of Event timer`` () = 
  let printer _ = printf "timer:   ms=%A\n" DateTime.Now.Millisecond
  let timer = new Timers.Timer(100.0) // ms
  
  timer.AutoReset <- true
  timer.Elapsed.Add printer
  
  let t = async {
      timer.Start()
      do! Async.Sleep 1000 // ms
      timer.Stop
  }
  Async.RunSynchronously t
  
  Assert.True(true)
  
[<Fact>]
let ``example of Event counter`` () =
  let mutable count = 0
  let counter _ =
    count <- count + 1
    printf "counter: count=%i\n" count
  let timer = new Timers.Timer(100.0) // ms
  
  timer.AutoReset <- true
  timer.Elapsed.Add counter
  
  let t = async {
      timer.Start()
      do! Async.Sleep 1000 // ms
      timer.Stop
  }
  Async.RunSynchronously t
  
  Assert.True(true)

let createTimer interval totalTime =
  let timer = new Timers.Timer(float interval)
  (
    async {
      timer.Start()
      do! Async.Sleep(totalTime)
      timer.Stop()
    },
    timer.Elapsed
  )

[<Fact>]
let ``example of Observable counter`` () =
  let mutable count = 0
  let inc _ =
    count <- count + 1
    count
    
  let task, source = createTimer 10 100
  let counter = source |> Observable.map inc
  
  counter
    |> Observable.subscribe (fun n -> printf "Observable: count=%i\n" n)
    |> ignore
  
  Async.RunSynchronously task
  