using System;

namespace Jcg.Application.Core.Optics
{
    public static class LensFactory
    {
        /// <summary>
        /// Creates a lens focused on a specific property of the root value using the provided getter and setter.
        /// The lens starts with the given root value. Each time the Value is set, a new root object is created via the setter.
        /// To access the updated root, use the RootValue property of the lens.
        /// </summary>
        /// <param name="initialRootValue">The initial root object.</param>
        /// <param name="getter">Function to extract the property value from the root object.</param>
        /// <param name="setter">Function to create a new root object with the updated property value.</param>
        /// <typeparam name="TRoot">Type of the root object.</typeparam>
        /// <typeparam name="TTarget">Type of the property.</typeparam>
        /// <returns>A configured lens instance.</returns>
        public static ILens<TRoot, TTarget> CreateLens<TRoot, TTarget>(
            this TRoot initialRootValue,
            Func<TRoot, TTarget> getter,
            Func<TRoot, TTarget, TRoot> setter)
        {
            return new Lens<TRoot, TTarget>(
                initialRootValue,
                getter,
                setter);
        }

        /// <summary>
        /// Creates a new lens focused on a nested property of the current lens's target,
        /// using the provided getter and setter for the nested property.
        /// This allows composition of lenses to access deeply nested or calculated values.
        /// </summary>
        /// <param name="previousLens">The existing lens focused on the source property.</param>
        /// <param name="getter">Function to extract the nested property value from the source object.</param>
        /// <param name="setter">Function to create a new source object with the updated nested property value.</param>
        /// <typeparam name="TRoot">Type of the root object.</typeparam>
        /// <typeparam name="TSource">Type of the intermediate property.</typeparam>
        /// <typeparam name="TTarget">Type of the nested property.</typeparam>
        /// <returns>A composed lens instance focused on the nested property.</returns>
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
}