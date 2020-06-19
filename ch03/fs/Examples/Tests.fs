module Tests

open System
open Program.FunctionalDataStructure
open Program.Calculator
open Xunit
open FsCheck
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

[<Property>]
let ``calculator works same as built in`` (a: NormalFloat, b: NormalFloat, c: NormalFloat, d: NormalFloat) =
  let a', b', c', d' = a.Get, b.Get, c.Get, d.Get
  let cal = Expr(Div,
              Expr(Add, Value(a'),
                Expr(Mul, Value(b'),
                  Expr(Sub, Value(c'), Value(d')))),
              Value(7.0)) // avoids division by 0
  let expected = ((a' + (b' * (c' - d'))) / 7.0)
  eval cal = expected