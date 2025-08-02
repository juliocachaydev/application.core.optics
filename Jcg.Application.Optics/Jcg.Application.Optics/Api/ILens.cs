using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Jcg.Application.Optics.Api
{
    public interface ILensState<TData, TProperty>
    {
        TData InitialData { get; }
        
        TData MutatedData { get; }
    }
    public interface ILens<TData, TProperty> : ILensState<TData, TProperty>
    {
        TProperty Value { get; set; }
    }
    
    public interface ICollectionLens<TData, TProperty> : ILensState<TData, IEnumerable<TProperty>>
    {
        IEnumerable<TProperty> Value { get; set; }

        void Remove();
    }

    public static class LensFactory
    {
        public static ILens<TData, TProperty> CreateLens<TData, TProperty>(
            this TData initialData,
            Func<TData, TProperty> selector,
            Func<TData, TProperty, TData> mutation)
        {
            throw new NotImplementedException();
        }
        
        
        public static ILens<TData, TProperty2> FocusProperty<TData, TProperty1, TProperty2>(
            this ILens<TData, TProperty1> currentLens,
            Func<TProperty1, TProperty2> selector,
            Func<TProperty1, TProperty2, TProperty1> mutation)
        {
            throw new NotImplementedException();
        }
        
        
        public static ICollectionLens<TData, TProperty> ToCollectionLens<TData, TProperty>(
            this ILens<TData, TProperty> currentLens,
            Func<TData, IEnumerable<TProperty>> selector,
            Func<TData, IEnumerable<TProperty>, TData> mutation)
        {
            throw new NotImplementedException();
        }
        
    }
}
