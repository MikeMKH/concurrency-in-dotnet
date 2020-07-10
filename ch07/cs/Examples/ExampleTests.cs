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
 
    }
}
