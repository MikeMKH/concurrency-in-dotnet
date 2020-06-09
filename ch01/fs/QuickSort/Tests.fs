module Tests =

  open System
  open Program
  open FsCheck
  open FsCheck.Xunit
  
  [<Property>]
  let ``should be same length`` (xs: int list) =
      (xs |> quicksort |> List.length) = (xs |> List.length)
  
  [<Property>]
  let ``should be in order`` (xs: int list) =
      xs |> quicksort |> Seq.pairwise |> Seq.forall(fun (x, y) -> x <= y)
