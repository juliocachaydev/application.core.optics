using System;

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
            RootObject = initialRootValue;
        }

        public TRoot RootObject { get; private set; }

        public TTarget Value
        {
            get => _getter(RootObject);
            set => RootObject = _setter(RootObject, value);
        }
    }
}