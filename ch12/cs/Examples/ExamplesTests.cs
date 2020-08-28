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
    }
}
