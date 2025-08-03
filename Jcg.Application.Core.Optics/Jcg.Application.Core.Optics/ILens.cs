namespace Jcg.Application.Core.Optics
{
    /// <summary>
    /// Abstracts a piece of data from the RootObject as a property (Value) with getter and setter,
    /// hiding the details of how the property is accessed or updated. The property can be direct,
    /// nested, or calculated. Setting the Value creates a new RootObject, supporting immutable
    /// records.
    /// </summary>
    /// <typeparam name="TRoot"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public interface ILens<TRoot, TTarget>
    {
        /// <summary>
        /// The current RootObject. This value is recreated each time the Value is set.
        /// </summary>
        TRoot RootObject { get; }

        /// <summary>
        /// The data abstracted as a property that you can get or set.
        /// </summary>
        TTarget Value { get; set; }
    }
}