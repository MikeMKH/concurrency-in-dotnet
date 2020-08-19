module AsyncExt
  type AsyncOption<'T> = Async<Option<'T>>
  
  let complexFunction(value: int): AsyncOption<int> =
    async {
      match (value % 2) with
      | 0 -> return Some(value)
      | 1 -> return None
    }
  let pure value = async.Return value
  let inline map (f: 'a->'b) (operation: Async<'a>) =
    async {
      let! result = operation
      return f result
    }
  let inline apply f op =
    async {
      let! fChild = Async.StartChild f
      let! opChild = Async.StartChild op
      let! fResult = fChild
      let! opResult = opChild
      return fResult opResult
    }
    
  let inline tap (f: 'a->'b) (x: Async<'a>) =
    map f x |> Async.Ignore |> Async.Start
    x
    
  let (<!>) = map
  let (<*>) = apply