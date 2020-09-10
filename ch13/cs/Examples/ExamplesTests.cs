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
    }
}
