module Program
  open System
  open System.IO
  open System.Net
  
  let httpAsync (url: string) = async {
    let req = WebRequest.Create(Uri(url))
    let! res = req.AsyncGetResponse()
    use stream = res.GetResponseStream()
    use reader = new StreamReader(stream)
    let! text = reader.ReadToEndAsync() |> Async.AwaitTask
    return text
  }