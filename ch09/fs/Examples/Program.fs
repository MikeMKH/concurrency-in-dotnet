module Program
  type MonadicLoggerBuilder(name: string) =
    member x.ReturnFrom(f) = f
    member x.Return(v) =
      printfn "Start Return %s" name
      let result = v
      printfn "End Return %s" name
      result
    member x.Delay(f) =
      printfn "Start Delay %s" name
      let result = f()
      printfn "End Delay %s" name
      result
    member x.Bind(v: 'a, cont: 'a -> 'b) =
      printfn "Start Bind %s" name
      let result = cont(v)
      printfn "End Bind %s" name
      result