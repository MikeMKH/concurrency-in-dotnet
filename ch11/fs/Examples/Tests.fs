module Tests

open System
open System.Threading
open Xunit

[<Fact>]
let ``MailboxProcessor i/o example`` () =
  let mutable spy = 0
  let agent = MailboxProcessor<string>.Start(fun inbox -> async {
    while true do
      let! message = inbox.Receive()
      spy <- message.Length
      printfn "MailboxProcessor: %s content-length=%d" message message.Length
  })
  
  let message = "hello"
  agent.Post message
  while spy = 0 do Thread.Sleep(10) // dangerous, but that is how I roll (not really but this is a kata)
  Assert.Equal(message.Length, spy)
  
[<Fact>]
let ``MailboxProcessor state example`` () =
  let mutable spy = 0
  let agent = MailboxProcessor<int>.Start(fun inbox -> async {
    let mutable sum = 0
    while true do
      let! value = inbox.Receive()
      sum <- value + sum
      spy <- sum
  })
  
  let postAndWait (ag: MailboxProcessor<'a>) (x: 'a) =
    ag.Post(x)
    Thread.Sleep(10) // might cause timing issues
  
  Assert.Equal(0, spy)
  postAndWait agent 8
  Assert.Equal(8, spy)
  postAndWait agent 5
  Assert.Equal(13, spy)
  postAndWait agent 7
  Assert.Equal(20, spy)
  postAndWait agent 22
  Assert.Equal(42, spy)

type message =
  | Name of string
  | Relay of AsyncReplyChannel<string>
  | Stop

[<Fact>]
let ``MailboxProcessor post and reply example`` () =
  let greeter = MailboxProcessor.Start(fun inbox -> async {
    let mutable name = "no man"
    while true do
      let! message = inbox.Receive()
      match message with
        | Name x -> name <- x
        | Relay(reply) -> reply.Reply(sprintf "Hello %s!" name)
        | Stop -> return() 
  })
  
  let mutable spy = ""
  
  spy <- greeter.PostAndReply(fun channel -> Relay channel)
  Assert.Equal("Hello no man!", spy)
  
  greeter.Post (Name "Mike")
  spy <- greeter.PostAndReply(fun channel -> Relay channel)
  Assert.Equal("Hello Mike!", spy)
  
  greeter.Post (Name "Mike")
  greeter.Post (Name "Kelsey")
  spy <- greeter.PostAndReply(fun channel -> Relay channel)
  Assert.Equal("Hello Kelsey!", spy)