module Program

open System.Collections.Concurrent

module Memoization =
    let memoize (f: 'a -> 'b) =
        let m = ConcurrentDictionary<'a,'b>()
        fun x ->   m.GetOrAdd(x, f)
