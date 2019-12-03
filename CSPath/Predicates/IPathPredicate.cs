namespace CSPath.Predicates
{
    /// <summary>
    /// Tests an object for a predicate and returns true or false. Used to filter objects
    /// from the input stream
    /// </summary>
    public interface IPathPredicate
    {
        /// <summary>
        /// Test the object against a known predicate and return true or false.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Test(object obj);
    }
}
