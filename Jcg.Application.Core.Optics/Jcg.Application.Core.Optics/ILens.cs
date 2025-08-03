using System;
using System.Collections.Generic;
using System.Linq;

namespace Jcg.Application.Core.Optics
{
    internal class Lens<TRoot, TTarget> : ILens<TRoot, TTarget>
    {
        private readonly Func<TRoot, TTarget> _getter;
        private readonly Func<TRoot, TTarget, TRoot> _setter;

        
        public Lens(
            TRoot initialRootValue,
            Func<TRoot, TTarget> getter,
            Func<TRoot, TTarget, TRoot> setter)
        {
            _getter = getter;
            _setter = setter;
            RootValue = initialRootValue;
        }
        
        public TRoot RootValue { get; private set; }

        public TTarget Value
        {
            get => _getter(RootValue);
            set => RootValue = _setter(RootValue, value);
        }
    }

    internal class FocusedLens<TRoot, TSource, TTarget> : ILens<TRoot, TTarget>
    {
        private readonly ILens<TRoot, TSource> _prevLens;
        private readonly Func<TSource, TTarget> _getter;
        private readonly Func<TSource, TTarget, TSource> _setter;

        public FocusedLens(
            ILens<TRoot, TSource> prevLens,
            Func<TSource, TTarget> getter,
            Func<TSource, TTarget, TSource> setter)
        {
            _prevLens = prevLens;
            _getter = getter;
            _setter = setter;
        }

        public TRoot RootValue => _prevLens.RootValue;

        public TTarget Value
        {
            get => _getter(_prevLens.Value);
            set => _prevLens.Value = _setter(_prevLens.Value, value);
        }
    }
    public interface ILens<TRoot, TTarget>
    {
        TRoot RootValue { get; }
        
        TTarget Value { get; set; }
    }

    public static class LensFactory
    {
        public static ILens<TRoot, TTarget> CreateLens<TRoot, TTarget>(
            this TRoot rootValue,
            Func<TRoot, TTarget> getter,
            Func<TRoot, TTarget, TRoot> setter)
        {
            return new Lens<TRoot, TTarget>(
                rootValue,
                getter,
                setter);
        }

        public static ILens<TRoot, TTarget> FocusLens<TRoot, TSource, TTarget>(
            this ILens<TRoot, TSource> previousLens,
            Func<TSource, TTarget> getter,
            Func<TSource, TTarget, TSource> setter)
        {
            return new FocusedLens<TRoot, TSource, TTarget>(
                previousLens,
                getter,
                setter);
        }
    }

    public static class LensCollectionExtensions
    {
        public static void AddWhenDoesNotExists<TRoot, TTarget>(
            this ILens<TRoot, IEnumerable<TTarget>> collectionLens,
            Func<TTarget, bool> existsFunction,
            Func<TTarget> factory)
        {
            if (collectionLens.Value.All(x => !existsFunction(x)))
            {
                collectionLens.Value = collectionLens.Value.Append(factory());
            }
        }
        
        public static void RemoveWhenExists<TRoot, TTarget>(
            this ILens<TRoot, IEnumerable<TTarget>> collectionLens,
            Func<TTarget, bool> existsFunction)
        {
            collectionLens.Value = collectionLens.Value.Where(x => !existsFunction(x));
        }
        
        public static void UpdateWhenExists<TRoot, TTarget>(
            this ILens<TRoot, IEnumerable<TTarget>> collectionLens,
            Func<TTarget, bool> predicate,
            Func<TTarget, TTarget> updateFunc)
        {
            collectionLens.Value = collectionLens.Value.Select(item =>
                predicate(item) ? updateFunc(item) : item);
        }
    }
}