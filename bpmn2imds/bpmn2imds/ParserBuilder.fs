#if INTERACTIVE
#r @"C:\Users\user\source\repos\DedAn\packages\BPMN.Sharp.1.0.6\lib\net20\BPMN.Sharp.dll"
#endif

namespace bpmn2imds

open BPMN

type ParsingWarning = 
    | ShapeBoundsCountsGreaterThanOne of elementId: string
    | CastingToGenericNode of elementId: string * elementType: string
    | ShapeBoundsCountNotEqualOne of elementId: string
    | AttachedToRefIsNotActivity of boundaryEventId: string * attachedToRef: string

type ParsingError =
    | ShapeElementRefNull of elementId: string
    | ShapePointsCountLessThanOne of elementId: string
    | ShapePointsNull of elementId: string
    | ElementNotFound of elementId: string
    | UknownFlowType of unknownFlowType: string
    | AttachedToRefNotFound of attachedToRef: string * eventId: string

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
        
    [<CustomOperation("validateEdge")>]
    member this.ValidateEdge (e: BPMN.Edge) = 
        if isNull e.ElementRef then
            Error (ShapeElementRefNull e.ID)
        else if isNull e.Points then
            Error (ShapePointsNull e.ID)
        else if e.Points.Count < 2 then
            Error (ShapePointsCountLessThanOne e.ID)
        else Ok (e, [])

    [<CustomOperation("validateShape")>]
    member this.ValidateShape (s: BPMN.Shape) = 
       if isNull s.ElementRef then
           Error (ShapeElementRefNull s.ID)
       else if isNull s.Bounds then
           Error (ShapePointsNull s.ID)
       else if s.Bounds.Count <> 1 then
           Ok ((s.ElementRef, Seq.head s.Bounds), [ShapeBoundsCountNotEqualOne s.ID])
       else Ok ((s.ElementRef, Seq.head s.Bounds), [])