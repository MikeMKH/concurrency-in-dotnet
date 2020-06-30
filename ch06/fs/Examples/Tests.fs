module Tests

open System
open System.Threading
open Xunit

[<Fact>]
let ``example of timer`` () = 
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
let ``example of counter`` () =
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