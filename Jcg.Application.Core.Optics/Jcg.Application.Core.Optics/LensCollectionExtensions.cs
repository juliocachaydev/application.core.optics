using System;
using System.Collections.Generic;
using System.Linq;

namespace Jcg.Application.Core.Optics
{
    public static class LensCollectionExtensions
    {
        /// <summary>
        /// Adds a new item to the collection if no existing item matches the specified condition.
        /// </summary>
        /// <typeparam name="TRoot">Type of the root object.</typeparam>
        /// <typeparam name="TTarget">Type of the collection item.</typeparam>
        /// <param name="collectionLens">Lens focused on the collection property.</param>
        /// <param name="existsFunction">Predicate to determine if an item exists.</param>
        /// <param name="factory">Function to create a new item if none exists.</param>
        public static void AddWhenDoesNotExists<TRoot, TTarget>(
            this ILens<TRoot, IEnumerable<TTarget>> collectionLens,
            Func<TTarget, bool> existsFunction,
            Func<TTarget> factory)
        {
            if (collectionLens.Value.All(x => !existsFunction(x)))
                collectionLens.Value = collectionLens.Value.Append(factory());
        }

        /// <summary>
        /// Removes items from the collection that match the specified condition.
        /// </summary>
        /// <typeparam name="TRoot">Type of the root object.</typeparam>
        /// <typeparam name="TTarget">Type of the collection item.</typeparam>
        /// <param name="collectionLens">Lens focused on the collection property.</param>
        /// <param name="existsFunction">Predicate to identify items to remove.</param>
        public static void RemoveWhenExists<TRoot, TTarget>(
            this ILens<TRoot, IEnumerable<TTarget>> collectionLens,
            Func<TTarget, bool> existsFunction)
        {
            collectionLens.Value = collectionLens.Value.Where(x => !existsFunction(x));
        }

        /// <summary>
        /// Updates items in the collection that match the specified condition using the provided update function.
        /// </summary>
        /// <typeparam name="TRoot">Type of the root object.</typeparam>
        /// <typeparam name="TTarget">Type of the collection item.</typeparam>
        /// <param name="collectionLens">Lens focused on the collection property.</param>
        /// <param name="predicate">Predicate to identify items to update.</param>
        /// <param name="updateFunc">Function to update matching items.</param>
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