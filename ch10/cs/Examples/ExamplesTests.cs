using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using Xunit;

namespace Examples
{
    public class ExamplesTests
    {
        [Fact]
        public async void ExampleAwaitAddAsync()
        {
            var a = await A();
            var b = await B();
            var c = await C();
            int result = a + b + c;
            Assert.Equal(1 + 2 + 3, result);
            
            async Task<int> A() { await Task.Delay(100); return 1; }
            async Task<int> B() { await Task.Delay(100); return 2; }
            async Task<int> C() { await Task.Delay(100); return 3; }
        }
        
        [Fact]
        public async void ExampleWhenAllAddAsync()
        {
            int result = (await Task.WhenAll(A(), B(), C())).Sum();
            Assert.Equal(1 + 2 + 3, result);
            
            async Task<int> A() { await Task.Delay(100); return 1; }
            async Task<int> B() { await Task.Delay(100); return 2; }
            async Task<int> C() { await Task.Delay(100); return 3; }
        }
        
        public async Task<string> FizzBuzzAsync(int value)
        {
            string result = (await Task.WhenAll(Fizz(value), Buzz(value))).Aggregate((m, s) => m + s);
            return  string.IsNullOrWhiteSpace(result) ? value.ToString() : result;
            
            async Task<string> Fizz(int n) { return n % 3 == 0 ? "Fizz" : null; }
            async Task<string> Buzz(int n) { return n % 5 == 0 ? "Buzz" : null; }
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(6)]
        [InlineData(15)]
        [InlineData(33)]
        [InlineData(333)]
        public async void FizzValueShouldFizzAsync(int value)
        {
            string result = await FizzBuzzAsync(value);
            Assert.Contains("Fizz", result);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(55)]
        [InlineData(555)]
        public async void BuzzValueShouldBuzzAsync(int value)
        {
            string result = await FizzBuzzAsync(value);
            Assert.Contains("Buzz", result);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(15)]
        [InlineData(30)]
        [InlineData(45)]
        [InlineData(1515)]
        [InlineData(151515)]
        public async void FizzBuzzValueShouldFizzBuzzAsync(int value)
        {
            string result = await FizzBuzzAsync(value);
            Assert.Equal("FizzBuzz", result);
        }
        
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(22)]
        [InlineData(2222)]
        public async void NonFizzBuzzValueShouldNotFizzNorBuzzAsync(int value)
        {
            string result = await FizzBuzzAsync(value);
            Assert.DoesNotContain("Fizz", result);
            Assert.DoesNotContain("Buzz", result);
            Assert.Equal(value.ToString(), result);
        }
        
        public int SumString(string s) =>
          (from letters in s select (int) letters).Sum();
          
        [Theory]
        [InlineData("", 0)]
        [InlineData(" ", 32)]
        [InlineData("a", 97)]
        [InlineData("A", 65)]
        [InlineData("AA", 65 * 2)]
        [InlineData("AA", 'A' + 'A')]
        [InlineData("Hello World", 72 + 101 + 108 + 108 + 111 + 32 + 87 + 111 + 114 + 108 + 100)]
        [InlineData("Hello World", 'H' + 'e' + 'l' + 'l' + 'o' + ' ' + 'W' + 'o' + 'r' + 'l' + 'd')]
        public void GivenStringMustSumToExpectedAmount(string str, int expected)
        {
            var actual = SumString(str);
            Assert.Equal(expected, actual);
        }
    }
}
