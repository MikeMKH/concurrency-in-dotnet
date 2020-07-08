using System;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Xunit;

namespace Examples
{
    public class ExampleTests
    {
        [Fact]
        public async void MonadicBindingTaskAsync()
        {
            Task<int> result = from task in Task.Run<int>(() => 40) select task + 2;
            
            int value = await result;
            Assert.Equal(42, value);
        }
        
        [Fact]
        public void AsyncSubjectPublishesOnlyLastValue()
        {
            ISubject<int> subject = new AsyncSubject<int>();
            int value = 0;
            
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnCompleted();
            subject.Subscribe(x => value = x);
            
            Assert.Equal(2, value);
        }
        
        [Fact]
        public void AsyncSubjectPublishesOnlyLastValueWhenYouSubscribeDoesNotMatter()
        {
            ISubject<int> subject = new AsyncSubject<int>();
            int r1 = 0;
            int r2 = 0;
            
            subject.Subscribe(x => r1 = x);
            
            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnCompleted();
            
            subject.Subscribe(x => r2 = x);
            
            Assert.Equal(2, r1);
            Assert.Equal(r1, r2);
        }
    }
}
