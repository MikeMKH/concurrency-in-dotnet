module OptionExt
  let pure value = Some value
  let map f op =
    match op with
    | Some x -> Some(f x)
    | None -> None
  let apply f op =
    match f, op with
    | Some g, Some x -> Some(g x)
    | _ -> None
  
  let (<!>) = map    
  let (<*>) = apply