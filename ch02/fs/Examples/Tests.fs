module Tests

open Program.Memoization
open Xunit
open FsCheck.Xunit

[<Property>]
let ``composition of math operations gives same result`` (n: int) =
  let add3 x = x + 3
  let mul4 x = x * 4
  (n |> (add3 >> mul4)) = mul4(add3 n)
    
[<Fact>]
let ``memoize functions are only called once for same argument`` () =
  let mutable counter = 0
  let _f x =
    counter <- counter + 1
    x
  let f = memoize _f
  
  f 1 |> ignore
  Assert.Equal(1, counter)
  
  f 1 |> ignore
  Assert.Equal(1, counter)
  
  f 8 |> ignore
  Assert.Equal(2, counter)
  
  f 8 |> ignore
  Assert.Equal(2, counter)
  
  f 1 |> ignore
  Assert.Equal(2, counter)
  