using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

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
    }
}
