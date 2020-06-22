module Tests

open Xunit
open FsCheck.Xunit
open PrimeNumbers

[<Theory>]
[<InlineData(2)>]
[<InlineData(3)>]
[<InlineData(5)>]
[<InlineData(7)>]
[<InlineData(11)>]
[<InlineData(13)>]
[<InlineData(17)>]
[<InlineData(19)>]
[<InlineData(23)>]
let ``prime numbers are prime`` (prime: int) =
    Assert.True(isPrime prime)

[<Theory>]
[<InlineData(-1)>]
[<InlineData(0)>]
[<InlineData(1)>]
[<InlineData(4)>]
[<InlineData(9)>]
[<InlineData(21)>]
let ``non-prime numbers are not prime`` (number: int) =
    Assert.False(isPrime number)

[<Property>]
let ``even numbers are not prime`` (n: int) =
  let number = if n = 1 then 2 else n
  Assert.False(2 * number |> isPrime)

[<Property>]
let ``composite numbers are not prime`` (number: int) =
  Assert.False(7 * 3 * number |> isPrime)