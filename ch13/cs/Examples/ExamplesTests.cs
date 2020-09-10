using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.ObjectPool;
using Xunit;

namespace Examples
{
    public class ExamplesTests
    {
        public class Hello
        {
            public string Value { get => "hello"; }
        }
        
        [Fact]
        public void ExampleObjectPool()
        {
            var policy = new DefaultPooledObjectPolicy<Hello>();
            var pool = new DefaultObjectPool<Hello>(policy);
            var hello = pool.Get();
            var value = hello.Value;
            pool.Return(hello);
            Assert.Equal("hello", hello.Value);
        }
    }
}
