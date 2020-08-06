module Tests

open System
open Xunit

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