module Tests

open System
open Program.FunctionalDataStructure
open Xunit
open FsCheck.Xunit

[<Property>]
let ``map identity FList gives equal FList`` (list: FList<int>) =
    map id list = list
    
[<Property>]
let ``map is composible`` (list: FList<int>) =
  map (fun x -> x * 2) list |> map (fun x -> x * 3) = map (fun x -> x * 2 * 3) list
  
[<Property>]
let ``folding two numbers with addition is the same as adding them`` (a: int, b: int) =
  let expected = a + b
  let list = Cons(a, Cons(b, Empty))
  fold (+) 0 list = expected
  
[<Property>]
let ``length of FList is same after map`` (list: FList<string>) =
  let uppercase (x: string) = if x <> null then x.ToUpper() else null
  map uppercase list |> length = (list |> length)
