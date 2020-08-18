module Tests

open System
open Xunit
open Program
open OptionExt

// based on https://fsharpforfunandprofit.com/posts/exceptions/

[<Fact>]
let ``System.Exception example`` () =
  let problem = fun () -> failwith "99 problems" |> ignore
  Assert.Throws<System.Exception>(problem)

[<Fact>]
let ``ArgumentException example`` () =
  let problem = fun () -> invalidArg "argument" "not valid and stuff" |> ignore
  Assert.Throws<ArgumentException>(problem)

[<Fact>]
let ``NullArgumentException example`` () =
  let problem = fun () -> invalidArg "argument" "is null and stuff" |> ignore
  Assert.Throws<ArgumentException>(problem)

[<Fact>]
let ``InvalidOperationException example`` () =
  let problem = fun () -> invalidOp "invalid operation and stuff" |> ignore
  Assert.Throws<InvalidOperationException>(problem)

[<Fact>]
let ``FieldAccessException C# sytle example`` () =
  let problem = fun () -> raise (FieldAccessException "your code's bad and you should feel bad") |> ignore
  Assert.Throws<FieldAccessException>(problem)

exception SomethingBadHappen of int

[<Fact>]
let ``custom exception example`` () =
  let problem = fun () -> raise (SomethingBadHappen 77) |> ignore
  Assert.Throws<SomethingBadHappen>(problem)
  
[<Theory>]
[<InlineData(2)>]
[<InlineData(4)>]
[<InlineData(44)>]
[<InlineData(4400)>] // rock solid testing
let ``given an even value complexFunction returns value`` (even: int) =
  let result = complexFunction(even) |> Async.RunSynchronously
  Assert.Equal(Some(even), result)
  
[<Theory>]
[<InlineData(1)>]
[<InlineData(11)>]
[<InlineData(13)>]
[<InlineData(1321)>] // again rock solid testing
let ``given an odd value complexFunction returns None`` (odd: int) =
  let result = complexFunction(odd) |> Async.RunSynchronously
  Assert.Equal(None, result)
  
[<Fact>]
let ``Async.Catch example with Choice1Of2`` () =
  let f = Async.Catch (async { return 1 })

  match f |> Async.RunSynchronously with
  | Choice1Of2 _ -> Assert.True(true)
  | Choice2Of2 _ -> Assert.True(false, "wrong result, it should not fail")
  
[<Fact>]
let ``Async.Catch example with Choice2Of2`` () =
  let f = Async.Catch (async { failwith "two" })

  match f |> Async.RunSynchronously with
  | Choice1Of2 _ -> Assert.True(false, "wrong result, it should fail")
  | Choice2Of2 _ -> Assert.True(true)
  
[<Fact>]
let ``Async.Catch end-to-end example`` () =
  let evenFail x = if x % 2 = 0 then failwith "even" else x
  let inc = (+) 1
  let printx = printf "Async.Catch: x=%i\n"
  
  let f = async { return 1 }
  let result =
    f
    |> tap printx
    |> map inc
    |> tap printx
    |> map evenFail
    |> Async.Catch
    
  match result |> Async.RunSynchronously with
  | Choice1Of2 _ -> Assert.True(false, "should fail")
  | Choice2Of2 _ -> Assert.True(true)
  
[<Fact>]
let ``apply applicative functor example`` () =
  let result = Some (+) <*> Some 1 <*> Some 2
  Assert.Equal(Some (1 + 2), result)
  
[<Fact>]
let ``applicative functor example`` () =
  let result = (+) <!> Some 1 <*> Some 2
  Assert.Equal(Some (1 + 2), result)