module Tests

open Program
open Xunit

[<Fact>]
let ``Example of parallel asynchronous computations`` () =
  let sites = [
      "http://www.google.com";          "http://www.amazon.com";
      "http://www.twitter.com";         "http://boardgamegeek.com";
      "http://dotnet.microsoft.com";    "http://www.anachronsounds.de";
      "http://www.github.com";          "http://www.againstme.net";
   ]
  
  sites
    |> Seq.map httpAsync
    |> Async.Parallel
    |> Async.RunSynchronously
    |> fun text -> Assert.True(text.Length > 0)
