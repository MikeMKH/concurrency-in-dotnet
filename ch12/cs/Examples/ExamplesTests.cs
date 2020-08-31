using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit;

namespace Examples
{
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
    }
}
