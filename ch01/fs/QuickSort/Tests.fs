module Tests =

  open Program
  open FsCheck.Xunit
  
  [<Property>]
  let ``should be same length`` (xs: int list) =
      (xs |> quicksort |> List.length) = (xs |> List.length)
  
  [<Property>]
  let ``should be in order`` (xs: int list) =
      xs |> quicksort |> Seq.pairwise |> Seq.forall(fun (x, y) -> x <= y)
  
  [<Property>]
  let ``should be idempotent`` (xs: int list) =
      (xs |> quicksort) = (xs |> quicksort |> quicksort)
  
  [<Property>]
  let ``should have same result`` (xs: int list) =
      (xs |> quicksort) = (xs |> quicksortWithDepth 3)
  
  [<Property>]
  let ``should be in order with max depth`` (xs: int list) =
      xs
        |> quicksortWithDepth ParallelismHelpers.MaxDepth
        |> Seq.pairwise |> Seq.forall(fun (x, y) -> x <= y)
  