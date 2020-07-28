using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Examples
{
    public class ExampleTests
    {
        Func<string, Action<byte[]>> test = (name) => (bytes) =>
        {
            var len = bytes.Length;
            Console.WriteLine(
                $"{name} id={Thread.CurrentThread.ManagedThreadId} read {len} bytes");
            Assert.True(bytes.Length > 0);
        };
        
        string CreateTestFile()
        {
            var testFile = Guid.NewGuid().ToString();
            using(var write = File.OpenWrite(testFile))
            {
                var data = "1\r\n2\r\n3\r\n";
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                write.Write(bytes);
            }
            return testFile;
        }
           
        [Fact]
        public void ExampleBlockingFileRead()
        {
            var testFile = CreateTestFile();   
            ReadFileBlocking(testFile, test("ReadFileBlocking"));
            
            void ReadFileBlocking(string path, Action<byte[]> process)
            {
                using(var read = new FileStream(
                    path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] buffer = new byte[read.Length];
                    var bytesRead = read.Read(buffer, 0, buffer.Length);
                    process(buffer);
                }
            }
        }
         
        [Fact]
        public void ExampleNonBlockingFileRead()
        {
            var testFile = CreateTestFile();   
            var result = ReadFileNonBlocking(testFile, test("ReadFileNonBlocking"));
            
            IAsyncResult ReadFileNonBlocking(string path, Action<byte[]> process)
            {
                using(var read = new FileStream(
                    path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, FileOptions.Asynchronous))
                {
                    byte[] buffer = new byte[read.Length];
                    var state = Tuple.Create(buffer, read, process);
                    var bytesRead = read.Read(buffer, 0, buffer.Length);
                    return read.BeginRead(buffer, 0, buffer.Length, EndReadCallback, state);
                }
                
                void EndReadCallback(IAsyncResult result)
                {
                    var state = result.AsyncState as Tuple<byte[], FileStream, Action<byte[]>>;
                    using(state.Item2) state.Item2.EndRead(result);
                    state.Item3(state.Item1);
                }
            }
        }
           
        [Fact]
        public void ExampleNonBlockingFileReadAsync()
        {
            var testFile = CreateTestFile();   
            ReadFileNonBlockingAsync(testFile, test("ReadFileNonBlockingAsync"));
            
            async void ReadFileNonBlockingAsync(string path, Action<byte[]> process)
            {
                using(var read = new FileStream(
                    path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, FileOptions.Asynchronous))
                {
                    byte[] buffer = new byte[read.Length];
                    var bytesRead = await read.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    await Task.Run(async () => process(buffer));
                }
            }
        }
        
        Func<string, Action<string>> PrintStateMessage = caller =>  state =>
              Console.WriteLine($"{caller} id={Thread.CurrentThread.ManagedThreadId} state={state}");

        [Fact]
        public void ExampleCancellationToken()
        {
            var print = PrintStateMessage("ExampleCancellationToken");
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;
            
            Task.Run(async () => 
            {
                var client = new WebClient();
                token.Register(() =>
                {
                    print("cancel");
                    client.CancelAsync();
                });
                var data = await client.DownloadDataTaskAsync("www.amazon.com");
            }, token);
            
            print("begin");
            Thread.Sleep(100);
            cancellationTokenSource.Cancel();
            print("end");
        }
        
        [Fact]
        public void ExampleCooperativeCancellationSupport()
        {
            var print = PrintStateMessage("ExampleCooperativeCancellationSupport");
            CancellationTokenSource cts1 = new CancellationTokenSource();
            CancellationTokenSource cts2 = new CancellationTokenSource();
            CancellationTokenSource composite = CancellationTokenSource.CreateLinkedTokenSource(
                cts1.Token, cts2.Token);
            
            var compositeToken = composite.Token;
            
            Task.Run(async () =>
            {
               print("start");
               Thread.Sleep(100);
               print("finish"); 
            }, compositeToken);
            
            print("before cancel");
            cts2.Cancel();
            print("after cancel");
        }
        
        [Fact]
        public async void ExampleDeferAsyncComputation()
        {
            Console.WriteLine("Defer: Start of Test");
            
            Task<int> eight = Task.Run(() => { Console.WriteLine("Defer: Running Task"); return 8; });
            Func<Task<int>> defer = async () => await eight;
            
            Console.WriteLine("Defer: Before Awaiting Task");
            var r1 = await eight;
            
            Console.WriteLine("Defer: Before Calling Func of Task");
            var t2 = defer();

            Console.WriteLine("Defer: Before Awaiting Func of Task");
            var r2 = await t2;
            Assert.Equal(r1, r2);
            
            Console.WriteLine("Defer: End of Test");
        }
        
        [Fact]
        public async void ExampleProcessingTaskAsTheyCompleteAsync()
        {
            int max = 10;
            var values = Enumerable.Range(1, max);
            var tasks = values.Select(
                value => Task.Run(() =>
                {
                    Console.WriteLine($"Async Processing id={Thread.CurrentThread.ManagedThreadId}\tvalue={value}");
                    Thread.Sleep(value * 10);
                    return value;
                })).ToList();
            
            int sum = 0;
            while(tasks.Count() > 0)
            {
                var task = await Task.WhenAny(tasks);
                sum += task.Result;
                tasks.Remove(task);
            }
            
            Assert.Equal(max * (max + 1) / 2, sum);
        }
    }
}
