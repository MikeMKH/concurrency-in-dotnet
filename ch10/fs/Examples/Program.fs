module Program
  type AsyncOption<'T> = Async<Option<'T>>
  
  let complexFunction(value: int): AsyncOption<int> =
    async {
      match (value % 2) with
      | 0 -> return Some(value)
      | 1 -> return None
    }
    
  let inline map (f: 'a->'b) (operation: Async<'a>) =
    async {
      let! result = operation
      return f result
    }
    
  let inline tap (f: 'a->'b) (x: Async<'a>) =
    map f x |> Async.Ignore |> Async.Start
    x