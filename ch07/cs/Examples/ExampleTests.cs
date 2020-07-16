using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Examples
{
    public class ExampleTests
    {
        [Fact]
        public void ExampleCreatingThreadsManually()
        {
            int x1 = 0, x2 = 0;
            var t1 = new Thread(() =>
            {
                Console.WriteLine($"Manual: Thread 1 on id={Thread.CurrentThread.ManagedThreadId}");
                x1 = 7;
            });
            var t2 = new Thread(() =>
            {
                Console.WriteLine($"Manual: Thread 2 on id={Thread.CurrentThread.ManagedThreadId}");
                x2 = 8;
            });
            
            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join();
            
            Assert.Equal(7, x1);
            Assert.Equal(8, x2);
        }
        
        [Fact]
        public void ExampleUsingThreadPoolQueueUserWorkItem()
        {
            int x1 = 0, x2 = 0;
            int returns = 0;
            
            ThreadPool.QueueUserWorkItem(x =>
            {
                Console.WriteLine($"ThreadPool: Thread 1 on id={Thread.CurrentThread.ManagedThreadId}");
                x1 = 7;
                returns++;
            });
            ThreadPool.QueueUserWorkItem(x =>
            {
                Console.WriteLine($"ThreadPool: Thread 2 on id={Thread.CurrentThread.ManagedThreadId}");
                x2 = 8;
                returns++;
            });
            
            while(returns != 2) Thread.Sleep(10); // wait for QueueUserWorkItem
            
            Assert.Equal(7, x1);
            Assert.Equal(8, x2);
        }
        
        [Fact]
        public void ExampleParallelInvoke()
        {
            int x1 = 0, x2 = 0, x10 = 0;
            int returns = 0;
            
            Parallel.Invoke(
                () => {
                    Console.WriteLine($"Parallel.Invoke: Thread 1 on id={Thread.CurrentThread.ManagedThreadId}");
                    x1 = 7;
                    returns++;
                },
                () => {
                    Console.WriteLine($"Parallel.Invoke: Thread 2 on id={Thread.CurrentThread.ManagedThreadId}");
                    x2 = 8;
                    returns++;
                },
                () => {
                    Console.WriteLine($"Parallel.Invoke: Thread 3 on id={Thread.CurrentThread.ManagedThreadId}");
                    returns++;
                },
                () => returns++,
                () => returns++,
                () => returns++,
                () => returns++,
                () => returns++,
                () => returns++,
                () => {
                    Console.WriteLine($"Parallel.Invoke: Thread 10 on id={Thread.CurrentThread.ManagedThreadId}");
                    x10 = 42;
                    returns++;
                }
            );
            
            while(returns != 10) Thread.Sleep(10);
            
            Assert.Equal(7, x1);
            Assert.Equal(8, x2);
            Assert.Equal(42, x10);
        }
        
        
        enum State
        {
            Init,
            Running,
            Done
        };
        
        [Fact]
        public void ExampleParallelInvokeWithStateMachine()
        {            
            int x1 = 0, x2 = 0;
            State t1 = State.Init, t2 = State.Init; 
            
            Parallel.Invoke(
                () => {
                    t1 = State.Running;
                    x1 = 7;
                    t2 = State.Done;
                },
                () => {
                    t2 = State.Running;
                    x2 = 8;
                    t2 = State.Done;
                }
            );
            
            while(t1 != State.Done && t2 != State.Done) Thread.Sleep(10);
            
            Assert.Equal(7, x1);
            Assert.Equal(8, x2);
        }
        
        [Fact]
        public void TestUnitEqualsUnit()
        {
            var x = Unit.Default;
            var y = Unit.Default;
            
            Assert.True(x.Equals(y));
            Assert.True(y.Equals(x));
            Assert.True(x == y);
            Assert.True(y == x);
            Assert.Equal(x, y);
        }
        
        [Fact]
        public void TestUnitDoesNotEqualAnythingElse()
        {
            string s = "";
            int i = default(int);
            Unit u = Unit.Default;
            
            Assert.False(u.Equals(s));
            Assert.False(s.Equals(u));
            
            Assert.False(u.Equals(i));
            Assert.False(i.Equals(u));
        }
        
        TResult Compute<TInput, TResult>(Task<TInput> task, Func<TInput, TResult> projection)
          => projection(task.Result);
        
        [Fact]
        public void TestComputeWithBooleanResult()
        {
            Task<int> t1 = Task.Run(() => 42);
            Task<int> t2 = Task.Run(() => 8);
         
            Func<int, bool> is42 = n => n == 42;
            Assert.True(Compute(t1, is42));
            Assert.False(Compute(t2, is42));
        }
        
        [Fact]
        public void TestComputeWithUnitResult()
        {
            Task<int> t1 = Task.Run(() => 42);
            Task<int> t2 = Task.Run(() => 8);
         
            bool was42 = false;
            Func<int, Unit> printAndStore = n =>
            {
                Console.WriteLine($"Unit: Does {n} == 42? {n == 42}");
                was42 = n == 42;
                return Unit.Default;
            };
            
            Compute(t1, printAndStore);
            Assert.True(was42);
            
            Compute(t2, printAndStore);
            Assert.False(was42);
        }
        
        [Fact]
        public void ExampleWrongParallelTask()
        {
            Random random = new Random(7);
            
            var valueTasks = 
              from value in Enumerable.Range(1, 10)
              select Task.Run(() => calculate(value));
            
            foreach(var task in valueTasks)
            {
                var value = task.Result;
                Console.WriteLine(
                    $"WrongParallelTask: id={Thread.CurrentThread.ManagedThreadId} value={value}");
            }
            
            int calculate(int value) => random.Next() * value;
        }
        
        [Fact]
        public void ExampleCorrectParallelTask()
        {
            ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(7));
            
            var valueTasks = 
              (from value in Enumerable.Range(1, 10)
              select Task.Run(() => calculate(value))).ToList();
            
            foreach(var task in valueTasks)
            {
                task.ContinueWith(t => Console.WriteLine(
                    $"CorrectParallelTask: id={Thread.CurrentThread.ManagedThreadId} value={t.Result}"));
            }
            
            int calculate(int value) => random.Value.Next() * value;
        }
        
        [Fact]
        public void TestComposibleTaskWithAddition()
        {
            var task =
              from x in Task.Run(() => 42)
              from y in Task.Run(() => x + 43)
              select y;
            
            Assert.Equal(85, task.Result);
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(8)]
        [InlineData(42)]
        public void TestTaskBindWithAddition(int value)
        {
            Func<int, Task<int>> plus2 = x => Task.FromResult(x +2);
            
            var task = Task.Run(() => value).Bind(plus2);
            
            Assert.Equal(value + 2, task.Result);
        }
        
        [Fact]
        public async void ExampleTraverseAsync()
        {
            // based on https://github.com/louthy/language-ext#transformer-types
            var start = DateTime.UtcNow;
            
            Func<int, int> createTask = n =>
            {
                Console.WriteLine($"Traverse: id={Thread.CurrentThread.ManagedThreadId} n={n}");
                Thread.Sleep(300);
                return n;    
            };
            
            var t1 = Task.Run(() => createTask(1));
            var t2 = Task.Run(() => createTask(2));
            var t3 = Task.Run(() => createTask(3));
            var t4 = Task.Run(() => createTask(4));
            var t5 = Task.Run(() => createTask(5));
            var t6 = Task.Run(() => createTask(6));
            var t7 = Task.Run(() => createTask(7));
            
            var result = await List(t1, t2, t3, t4, t5, t6, t7).Traverse(x => x * 2);
            Assert.True(toSet(result) == Set(2, 4, 6, 8, 10, 12, 14));
            
            var ms = (int)(DateTime.UtcNow - start).TotalMilliseconds;
            Assert.True(ms < 400, $"Took {ms} ticks");
            
            
        }
    }
}
