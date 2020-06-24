module Tests

open System
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

[<Theory>]
[<InlineData(2, 2)>]
[<InlineData(3, 5)>]
[<InlineData(4, 5)>]
[<InlineData(5, 10)>]
[<InlineData(6, 10)>]
[<InlineData(7, 17)>]
[<InlineData(20, 77)>]
[<InlineData(100, 1060)>]
let ``given primes of 0..top sums to expected`` (top: int, expected: int) =
  Assert.Equal(expected, sequentialSum top)
  
[<Property>]
let ``sequential sum and parallel sum give same result`` (n: int) =
  let top = Math.Abs(n) % 1000
  let sequential = sequentialSum top
  let parallel = parallelSum top
  Assert.Equal(sequential, parallel)
  
[<Property>]
let ``sequential sum and parallel Linq sum give same result`` (n: int) =
  let top = Math.Abs(n) % 1000
  let sequential = sequentialSum top
  let parallel = parallelLinqSum top
  Assert.Equal(sequential, parallel)