using System;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Xunit;

namespace Examples
{
    public class ExampleTests
    {
        [Fact]
        public async void MonadicBindingTask()
        {
            Task<int> result = from task in Task.Run<int>(() => 40) select task + 2;
            
            int value = await result;
            Assert.Equal(42, value);
        }
    }
}
