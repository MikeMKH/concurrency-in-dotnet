using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Examples
{
    public class Example
    {
        static ConcurrentDictionary<int, int> dictionary = new ConcurrentDictionary<int, int>();
        
        [Fact]
        public void ConcurrentDictionaryIsThreadSafe()
        {
            var t1 = Task.Run(async () => dictionary.TryAdd(1, 1));
            var t2 = Task.Run(async () => dictionary.TryAdd(2, 2));
            var t3 = Task.Run(async () => dictionary.TryAdd(3, 3));
            var t4 = Task.Run(async () => dictionary.TryAdd(4, 4));
            
            Task.WaitAll(t1, t2, t3, t4);
            Assert.Equal(4, dictionary.Count());
        }
    }
}
