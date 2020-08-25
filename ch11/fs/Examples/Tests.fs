module Tests

open System
open System.Threading
open Xunit

type NameMessage =
  | Name of string
  | Relay of AsyncReplyChannel<string>
  | Stop

type CountMessage =
  | Increment of int
  | Sum of AsyncReplyChannel<int>
  
type EchoMessage<'a> =
  | Echo of 'a * AsyncReplyChannel<'a>
  
type FizzBuzzMessage =
  | TestFizzBuzzer of int * AsyncReplyChannel<string option>

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
  
  spy <- greeter.PostAndReply Relay
  Assert.Equal("Hello no man!", spy)
  
  Name "Mike" |> greeter.Post
  spy <- greeter.PostAndReply Relay
  Assert.Equal("Hello Mike!", spy)
  
  Name "Mike" |> greeter.Post
  Name "Kelsey" |> greeter.Post
  spy <- greeter.PostAndReply Relay
  Assert.Equal("Hello Kelsey!", spy)
  
  greeter.Post Stop
  
[<Fact>]
let ``MailboxProcessor post and reply recursive loop example`` () =
  let counter = MailboxProcessor.Start(fun inbox ->
    let rec loop sum = async {
      let! message = inbox.Receive()
      match message with
      | Increment x -> return! sum + x |> loop
      | Sum(reply) -> reply.Reply(sum); return! loop sum
    }
    loop 0
  )
  
  Assert.Equal(0, counter.PostAndReply Sum)
  Assert.Equal(0, counter.PostAndReply Sum)
  
  Increment 8 |> counter.Post
  Assert.Equal(8, counter.PostAndReply Sum)
  Assert.Equal(8, counter.PostAndReply Sum)
  
  Increment 7 |> counter.Post
  Assert.Equal(15, counter.PostAndReply Sum)
  
  Increment 11 |> counter.Post
  Assert.Equal(26, counter.PostAndReply Sum)
  
[<Fact>]
let ``EchoMessage can post and reply in same call`` () =
  let echo = MailboxProcessor.Start(fun inbox ->
    let rec loop = async {
      let! Echo(value, reply) = inbox.Receive()
      reply.Reply(value)
      return! loop
    }
    loop
  )
  
  Assert.Equal("Hello world", echo.PostAndReply(fun channel -> Echo("Hello world", channel)))
  Assert.Equal("echo", echo.PostAndReply(fun channel -> Echo("echo", channel)))
  
[<Fact>]
let ``FizzBuzzMessage can post and reply with Optional type`` () =
  let fizzer = MailboxProcessor.Start(fun inbox ->
    let rec loop = async {
      let! TestFizzBuzzer(value, reply) = inbox.Receive()
      match value % 3 = 0 with
      | true -> reply.Reply(Some("fizz"))
      | false -> reply.Reply(None)
      return! loop
    }
    loop
  )
  
  Assert.Equal(Some("fizz"), fizzer.PostAndReply(fun channel -> TestFizzBuzzer(0, channel)))
  Assert.Equal(Some("fizz"), fizzer.PostAndReply(fun channel -> TestFizzBuzzer(3, channel)))
  Assert.Equal(Some("fizz"), fizzer.PostAndReply(fun channel -> TestFizzBuzzer(9, channel)))
  Assert.Equal(Some("fizz"), fizzer.PostAndReply(fun channel -> TestFizzBuzzer(15, channel)))
  Assert.Equal(None, fizzer.PostAndReply(fun channel -> TestFizzBuzzer(2, channel)))
  Assert.Equal(None, fizzer.PostAndReply(fun channel -> TestFizzBuzzer(5, channel)))
  