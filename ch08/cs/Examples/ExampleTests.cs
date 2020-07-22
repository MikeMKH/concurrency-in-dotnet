using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Examples
{
    public class ExampleTests
    {
        Action<byte[]> test = (bytes) =>
        {
            var len = bytes.Length;
            Console.WriteLine(
                $"id={Thread.CurrentThread.ManagedThreadId} read {len} bytes");
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
            ReadFileBlocking(testFile, test);
            
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
            var result = ReadFileNonBlocking(testFile, test);
            
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
            ReadFileNonBlockingAsync(testFile, test);
            
            async void ReadFileNonBlockingAsync(string path, Action<byte[]> process)
            {
                using(var read = new FileStream(
                    path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, FileOptions.Asynchronous))
                {
                    byte[] buffer = new byte[read.Length];
                    var bytesRead = await read.ReadAsync(buffer, 0, buffer.Length);
                    await Task.Run(async () => process(buffer));
                }
            }
        }
    }
}
