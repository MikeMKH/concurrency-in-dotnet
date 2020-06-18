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
