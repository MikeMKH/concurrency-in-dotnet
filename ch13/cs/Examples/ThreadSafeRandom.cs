using System;
using System.Threading;

namespace Examples
{
  public class ThreadSafeRandom : Random
  {
    private ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(MakeNextSeed()));
    static int MakeNextSeed() => Guid.NewGuid().ToString().GetHashCode();
    
    public override int Next() => random.Value.Next();
    public override int Next(int maxValue) => random.Value.Next(maxValue);
    public override int Next(int minValue, int maxValue) => random.Value.Next(minValue, maxValue);
    public override double NextDouble() => random.Value.NextDouble();
    public override void NextBytes(byte[] buffer) => random.Value.NextBytes(buffer);
      
  }
}