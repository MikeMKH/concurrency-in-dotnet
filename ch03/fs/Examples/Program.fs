module Program

module FunctionalDataStructure =
  type FList<'a> =
    | Empty
    | Cons of head: 'a * tail: FList<'a>
    
  let rec map f list: FList<'a> =
    match list with
    | Empty -> Empty
    | Cons(hd, tl) -> Cons(f hd, map f tl)
    
  let rec fold f (m: 'b) (list: FList<'a>) =
    match list with
    | Empty -> m
    | Cons(hd, tl) -> fold f (f m hd) tl
    
  let length list = fold (fun m _ -> m + 1) 0 list
  
module Calculator =

  open System.Threading.Tasks
  
  type Operation =
    | Add
    | Sub
    | Mul
    | Div
    
  and Calculator =
    | Value of double
    | Expr of Operation * Calculator * Calculator
  
  let spawn (op: unit->double) = Task.Run(op)
  
  let rec eval (cal: Calculator) =
    match cal with
    | Value(x) -> x
    | Expr(op, lt, rt) ->
        let r1 = spawn(fun () -> eval lt)
        let r2 = spawn(fun () -> eval rt)
        let apply = Task.WhenAll([r1; r2])
        let ltRes, rtRes = apply.Result.[0], apply.Result.[1]
        match op with
        | Add -> ltRes + rtRes
        | Sub -> ltRes - rtRes
        | Mul -> ltRes * rtRes
        | Div -> ltRes / rtRes
