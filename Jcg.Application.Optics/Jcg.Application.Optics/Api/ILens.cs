using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Jcg.Application.Optics.Api
{
    
    public interface ILens<TRoot, TTarget>
    {
        TRoot UpdatedRoot { get; }
        
        TTarget Value { get; set; }
    }
    

    public static class LensFactory
    {
        public static ILens<TRoot, TTarget> CreateLens<TRoot, TTarget>(
            this TRoot source,
            Func<TRoot, TTarget> getter,
            Func<TRoot, TTarget, TRoot> setter)
        {
            throw new NotImplementedException();
        }

        public static ILens<TRoot, TTarget> FocusLens<TRoot, TSource, TTarget>(
            this ILens<TRoot, TSource> lens,
            Func<TSource, TTarget> getter,
            Func<TSource, TTarget, TSource> setter)
        {
            throw new NotImplementedException();
        }
    }
}