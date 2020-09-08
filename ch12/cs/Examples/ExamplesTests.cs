using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace Examples
{
    interface IAgent<TMessage>
    {
        Task Send(TMessage message);
        void Post(TMessage message);
    }
    
    class StatefulAgent<TState, TMessage> : IAgent<TMessage>
    {
        private TState state;
        private readonly ActionBlock<TMessage> actionBlock;
        
        public StatefulAgent(
            TState initialState, Func<TState, TMessage, Task<TState>> action, CancellationToken? token = null)
        {
            state = initialState;
            var options = new ExecutionDataflowBlockOptions { 
                CancellationToken = token.HasValue ? token.Value : CancellationToken.None };
            actionBlock = new ActionBlock<TMessage>(
                async message => state = await action(state, message), options);
        }
        
        public Task Send(TMessage message) => actionBlock.SendAsync(message);
        public void Post(TMessage message) => actionBlock.Post(message);
        public TState Result()
        {
            actionBlock.Complete();
            actionBlock.Completion.Wait();
            return state;
        }
    }
    
    public class ExamplesTests
    {
        [Fact]
        public async void ExampleBufferBlockAsync()
        {
            BufferBlock<int> buffer = new BufferBlock<int>();
            
            IEnumerable<int> values = Enumerable.Range(1, 100);
            await Task.WhenAll(
                ProducerAsync(values),
                ConsumerAsync(x => Assert.True(x > 0)));
            
            async Task ProducerAsync(IEnumerable<int> values)
            {
                foreach(var value in values)
                  buffer.Post(value);
                buffer.Complete();
            }
            
            async Task ConsumerAsync(Action<int> process)
            {
                while(await buffer.OutputAvailableAsync())
                  process(await buffer.ReceiveAsync());
            }
        }
        
        [Fact]
        public async void ExampleTransformBlockWithActionBlockAsync()
        {
            var results = new List<bool>();
            TransformBlock<int, string> transform = new TransformBlock<int, string>(
                async value => value % 3 == 0 ? "Fizz" : "");
            ActionBlock<string> isFizz = new ActionBlock<string>(
                async result => results.Add(result == "Fizz"));
            
            transform.LinkTo(isFizz, new DataflowLinkOptions { PropagateCompletion = true });
            
            transform.Post(3);
            transform.Post(33);
            transform.Post(333);
            transform.Post(0);
            transform.Post(15);
            transform.Post(6);
            transform.Post(9);
            
            transform.Complete();
            transform.Completion.Wait();
            
            Assert.DoesNotContain(false, results);
        }
        
        [Fact]
        public async void ExampleMultipleTransformBlockWithActionBlockAsync()
        {
            var results = new List<bool>();
            TransformBlock<int, (int, string)> start = new TransformBlock<int, (int, string)>(
                async value => (value, ""));
            TransformBlock<(int, string), (int, string)> fizz = new TransformBlock<(int, string), (int, string)>(
                async t => (t.Item1, (t.Item2 + (t.Item1 % 3 == 0 ? "Fizz" : ""))));
            TransformBlock<(int, string), (int, string)> buzz = new TransformBlock<(int, string), (int, string)>(
                async t => (t.Item1, (t.Item2 + (t.Item1 % 5 == 0 ? "Buzz" : ""))));
            ActionBlock<(int, string)> isFizzBuzz = new ActionBlock<(int, string)>(
                async t => results.Add(t.Item2 == "FizzBuzz"));
            
            start.LinkTo(fizz, new DataflowLinkOptions { PropagateCompletion = true });
            fizz.LinkTo(buzz, new DataflowLinkOptions { PropagateCompletion = true });
            buzz.LinkTo(isFizzBuzz, new DataflowLinkOptions { PropagateCompletion = true });
            
            start.Post(0);
            start.Post(15);
            start.Post(45);
            start.Post(1515);
            
            start.Complete();
            start.Completion.Wait();
            
            Assert.DoesNotContain(false, results);
        }
        
        [Fact]
        public async void ExampleMultipleProducersAsync()
        {
            BufferBlock<int> buffer = new BufferBlock<int>(
                new DataflowBlockOptions { BoundedCapacity = 10 }
            );
            
            int sum = 0;
            
            int UP_TO = 20;
            IEnumerable<int> values = Enumerable.Range(0, UP_TO);
            
            Console.WriteLine($"MultipleProducers: processors={Environment.ProcessorCount}");
            await Task.WhenAll(
                MultipleProducers(values, values, values),
                Consumer(
                    n => {
                        sum += n; // would not work with lots of threads
                        Console.WriteLine(
                        $"MultipleProducers: n={n} thread={Thread.CurrentThread.ManagedThreadId}");
                    })
            );
            
            Assert.Equal(((UP_TO * (UP_TO - 1)) / 2) * 3, sum);
            
            async Task Produce(IEnumerable<int> values)
            {
                foreach(var value in values)
                  await buffer.SendAsync(value);
            }
            
            async Task MultipleProducers(params IEnumerable<int> [] producers)
              => await Task.WhenAll(
                  (from producer in producers select Produce(producer)).ToArray())
                  .ContinueWith(_ => buffer.Complete());
            
            async Task Consumer(Action<int> process)
            {
                while(await buffer.OutputAvailableAsync())
                  process(await buffer.ReceiveAsync());
            }
        }
        
        [Fact]
        public void ExampleStatefulAgentUsedToKeepSum()
        {
            var agent = new StatefulAgent<int, int>(0, (m, n) => Task.Run(() => m + n));
            
            agent.Post(1);
            agent.Send(2);
            agent.Post(3);
            
            Assert.Equal(6, agent.Result());
        }
        
        [Fact]
        public void ExampleStatefulAgentUsedToWordCount()
        {
            var agent = new StatefulAgent<ImmutableDictionary<string, int>, string>(
                ImmutableDictionary<string, int>.Empty,
                async (m, w) => {
                    var word = w.ToLower();
                    if(m.TryGetValue(word, out int count))
                      return m.SetItem(word, count + 1);
                    
                    return m.Add(word, 1);
                }
            );
            
            agent.Post("hello");
            agent.Post("world");
            agent.Post("Hello");
            agent.Post("HELLO");
            agent.Post("worlD");
            
            var counts = agent.Result();
            Assert.Equal(3, counts["hello"]);
            Assert.Equal(2, counts["world"]);
            counts.TryGetValue("not found", out int notFound);
            Assert.Equal(0, notFound);
        }
    }
}
