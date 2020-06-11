module Tests

open System
open Xunit
open FsCheck.Xunit

[<Property>]
let ``composition of math operations gives same result`` (n: int) =
    let add3 x = x + 3
    let mul4 x = x * 4
    (n |> (add3 >> mul4)) = mul4(add3 n)