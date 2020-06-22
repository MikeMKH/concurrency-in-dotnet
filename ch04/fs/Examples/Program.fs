module PrimeNumbers

  open System
  
  let isPrime n =
    if n < 2 then false
    elif n = 2 then true
    else
      let boundary = int(Math.Floor(Math.Sqrt(float(n))))
      [2..boundary]
        |> Seq.exists(fun x -> n % x = 0)
        |> not