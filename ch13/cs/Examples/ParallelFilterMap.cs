using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelFilterMapEx
{
  public static class ParallelFilterMapExtension
  {
    public static IList<TOuput> ParallelFilterMap<TInput, TOuput>(this IList<TInput> input,
      Func<TInput, bool> predicate, Func<TInput, TOuput> transform, ParallelOptions options = null)
    {
      options = options ?? new ParallelOptions();
      var syncResults = ArrayList.Synchronized(new List<IList<TOuput>>());
      
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
          local => syncResults.Add(local)
        );
        
        var results = new List<IList<TOuput>>();
        lock(syncResults.SyncRoot)
        {
          foreach(var r in syncResults)
            results.Add(r as IList<TOuput>);
        }
        
        return  results.SelectMany(x => x).ToList();
    }
  }
}