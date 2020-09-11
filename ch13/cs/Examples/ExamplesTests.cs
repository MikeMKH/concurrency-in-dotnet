using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using Xunit;

namespace Examples
{
    public class ExamplesTests
    {
        public class Hello
        {
            public Hello() => _value = "hello";
            private string _value;
            public string Value { get => _value; set => _value = value; }
        }
        
        [Fact]
        public void ExampleObjectPool()
        {
            var policy = new DefaultPooledObjectPolicy<Hello>();
            var pool = new DefaultObjectPool<Hello>(policy);
            var hello = pool.Get();
            var value = hello.Value;
            pool.Return(hello);
            Assert.Equal("hello", value);
        }
        
        [Fact]
        public void TestObjectPoolReturnsSameObject()
        {
            var policy = new DefaultPooledObjectPolicy<Hello>();
            var pool = new DefaultObjectPool<Hello>(policy);
            
            var obj1 = pool.Get();
            var hello = obj1.Value;
            obj1.Value = "bye";
            pool.Return(obj1);
            
            var obj2 = pool.Get();
            var bye = obj2.Value;
            pool.Return(obj2);
            
            Assert.Equal("hello", hello);
            Assert.Equal("bye", bye);
        }
        
        [Fact]
        public void ExampleThreadSafeRandom()
        {
            var random = new ThreadSafeRandom();
            string [] clips = new string[] { "1.mp3", "2.mp3", "3.mp3" };
            
            Parallel.For(0, 1000, (n) =>
            {
                var index = random.Next(3);
                var clip = clips[index];
                Console.WriteLine($"ThreadSafeRandom: clip={clip} thread={Thread.CurrentThread.ManagedThreadId}");
            });
            
            Assert.True(random.Next(3) <= 3);
        }
        
        [Theory]
        [InlineData(1, 1)]
        [InlineData(-1, 1)]
        [InlineData(0, 0)]
        [InlineData(100, 101)]
        [InlineData(0, 100)]
        [InlineData(22, 23)]
        [InlineData(2929, 29399)]
        public void GivenMinMaxThreadSafeRandomShouldReturnValueBetweenMinMax(int min, int max)
        {
            var random = new ThreadSafeRandom();
            var value = random.Next(min, max);
            Assert.True(value >= min);
            Assert.True(value <= max);
        }
    }
}
