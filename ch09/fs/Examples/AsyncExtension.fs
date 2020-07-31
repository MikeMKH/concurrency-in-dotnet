module AsyncExtension
  let inline map (f: 'a->'b) (operation: Async<'a>) =
    async {
      let! result = operation
      return f result
    }
    
  let inline tap (f: 'a->'b) (x: Async<'a>) =
    map f x |> Async.Ignore |> Async.Start
    x