using System;
using System.Threading;
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
    }
}
