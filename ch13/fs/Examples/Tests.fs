module Tests

open Xunit

let bind (operation: 'a->Async<'b>) (x: Async<'a>) =
  async {
    let! r = x
    return! operation r
  }

let (>>=) (x: Async<'a>) (operation: 'a->Async<'b>) =
  bind operation x

let kleisli (f: 'a->Async<'b>) (g: 'b->Async<'c>) (x: 'a) =
  (f x) >>= g

let (>=>) (f: 'a->Async<'b>) (g: 'b->Async<'c>) (x: 'a) =
  kleisli f g x

[<Fact>]
let ``given lifted function we can compose with kleisli`` () =
  let kasync x = fun y -> async { return x }
  
  let f = kasync "Hello"
  let g = kasync 8
  let h = f >=> g
  
  let actual = h 9.0 |> Async.RunSynchronously
  
  Assert.Equal(8, actual)
