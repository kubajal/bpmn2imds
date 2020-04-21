#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

namespace bpmn2imds

open BPMN

type ParserBuilder() =
    member this.Bind(m, f) =
        Result.bind f m
    
    member this.Return(x) =
        Ok (x, [])
    
    member this.For(list, f) = list |> Seq.map(fun e -> f e)
        
    member this.Yield(x) = 
        Ok (x, [])

    member this.YieldFrom(x) = 
        x

    member this.Zero() =
        None
    
    [<CustomOperation("apply")>]
    member this.Apply(x, fs) =
        match fs with 
        | f :: tail -> this.Apply(this.Bind(x, f), tail)
        | [] -> x