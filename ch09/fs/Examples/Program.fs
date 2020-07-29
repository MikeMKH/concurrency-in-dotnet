module AsyncExtension
  let inline map (f: 'a->'b) (operation: Async<'a>) =
    async {
      let! result = operation
      return f result
    }