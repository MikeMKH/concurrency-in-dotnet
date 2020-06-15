module Tests

open Program.Memoization
open Program.ConsurrentSpeculation
open Xunit
open FsCheck.Xunit

[<Property>]
let ``composition of math operations gives same result`` (n: int) =
  let add3 x = x + 3
  let mul4 x = x * 4
  (n |> (add3 >> mul4)) = mul4(add3 n)
    
[<Fact>]
let ``memoize functions are only called once for same argument`` () =
  let mutable counter = 0
  let _f x =
    counter <- counter + 1
    x
  let f = memoize _f
  
  f 1 |> ignore
  Assert.Equal(1, counter)
  
  f 1 |> ignore
  Assert.Equal(1, counter)
  
  f 8 |> ignore
  Assert.Equal(2, counter)
  
  f 8 |> ignore
  Assert.Equal(2, counter)
  
  f 1 |> ignore
  Assert.Equal(2, counter)

[<Fact>]
let ``fuzzy match will find exact match at beginning`` () =
  let numberMatch = fuzzyMatch [1; 2; 3]
  let expected = 1
  let actual = numberMatch expected
  Assert.Equal(expected, actual)

[<Fact>]
let ``fuzzy match will find exact match at end`` () =
  let numberMatch = fuzzyMatch [1; 2; 3]
  let expected = 3
  let actual = numberMatch expected
  Assert.Equal(expected, actual)

[<Fact>]
let ``fuzzy match will find nearest matches below value`` () =
  let numberMatch = fuzzyMatch [3; 7; 30; 4; 100]
  let expected = 4
  let actual = numberMatch (expected + 1)
  Assert.Equal(expected, actual)

[<Fact>]
let ``fuzzy match will find nearest matches above value`` () =
  let numberMatch = fuzzyMatch [3; 7; 30; 6; 100]
  let expected = 6
  let actual = numberMatch (expected - 1)
  Assert.Equal(expected, actual)