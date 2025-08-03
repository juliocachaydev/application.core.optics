using System;

namespace Jcg.Application.Core.Optics
{
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

        public TRoot RootObject => _prevLens.RootObject;

        public TTarget Value
        {
            get => _getter(_prevLens.Value);
            set => _prevLens.Value = _setter(_prevLens.Value, value);
        }
    }
}