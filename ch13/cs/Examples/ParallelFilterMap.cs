using System;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelFilterMapEx
{
  public static class ParallelFilterMapExtension
  {
    public static TOuput[] ParallelFilterMap<TInput, TOuput>(this IList<TInput> input,
      Predicate<TInput> predicate, Func<TInput, TOuput> transform, ParallelOptions options = null)
    {
      options = options ?? new ParallelOptions();
      var results = ImmutableList<List<TOuput>>.Empty;
      
      Parallel.ForEach(
        input,
        options,
        () => new List<TOuput>(),
        delegate (
          TInput item,
          ParallelLoopState state,
          List<TOuput> local)
          {
            if(predicate(item))
              local.Add(transform(item));
            
            return local;
          },
          local => results.Add(local)
        );
        
        return results.SelectMany(x => x).ToArray();
    }
  }
}