namespace CSPath
{
    /// <summary>
    /// Wraps a value for passing between IPath instances
    /// Concrete implementations may include additional contextual information
    /// </summary>
    public interface IValueWrapper
    {
        /// <summary>
        /// the value being passed
        /// </summary>
        object Value { get; }
    }
}
