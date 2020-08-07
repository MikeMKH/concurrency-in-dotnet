module Program
  type AsyncOption<'T> = Async<Option<'T>>
  
  let complexFunction(value: int): AsyncOption<int> =
    async {
      match (value % 2) with
      | 0 -> return Some(value)
      | 1 -> return None
    }
