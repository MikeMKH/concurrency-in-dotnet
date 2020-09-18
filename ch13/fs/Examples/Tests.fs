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
let ``given lifted function we can compose K with kleisli`` () =
  let k x = fun y -> async { return x }
  let f = k "Hello"
  let g = k 8
  let h = f >=> g
  
  let actual = h 9.0 |> Async.RunSynchronously
  
  Assert.Equal(8, actual)

[<Fact>]
let ``given lifted function we can compose I with kleisli`` () =
  let i x = async { return x }
  let h = i >=> i
  
  let actual = h "Hello" |> Async.RunSynchronously
  
  Assert.Equal("Hello", actual)

[<Fact>]
let ``given lifted function we can compose option with kleisli`` () =
  let f x = async { return Some x }
  let g x = async {
    return match x with
           | Some _ -> "got it"
           | None -> "nothing" 
  }
  let h = f >=> g
  
  let actual = h "Hello" |> Async.RunSynchronously
  
  Assert.Equal("got it", actual)