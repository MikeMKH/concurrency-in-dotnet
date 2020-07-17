module Tests

open System
open System.IO
open System.Net
open Xunit

[<Fact>]
let ``Example of parallel asynchronous computations`` () =
  let httpAsync (url: string) = async {
    let req = WebRequest.Create(Uri(url))
    let! res = req.AsyncGetResponse()
    use stream = res.GetResponseStream()
    use reader = new StreamReader(stream)
    let! text = reader.ReadToEndAsync() |> Async.AwaitTask
    return text
  }
  
  let sites = [ "http://www.google.com"; "http://www.amazon.com" ]
  
  sites
    |> Seq.map httpAsync
    |> Async.Parallel
    |> Async.RunSynchronously
    |> fun text -> Assert.True(text.Length > 0)
