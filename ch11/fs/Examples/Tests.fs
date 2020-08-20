module Tests

open System
open System.Threading
open Xunit

[<Fact>]
let ``Mailbox Processor example`` () =
  let mutable size = 0
  let agent = MailboxProcessor<string>.Start(fun inbox -> async {
    while true do
      let! message = inbox.Receive()
      size <- message.Length
      printfn "MailboxProcessor: %s content-length=%d" message message.Length
  })
  
  let message = "hello"
  agent.Post message
  while size = 0 do Thread.Sleep(10) // dangerous, but that is how I roll (not really but this is a kata)
  Assert.Equal(message.Length, size)
  
  
