namespace DocumentParser;

/// <summary>
/// Interface for the DocumentReviver
/// </summary>
/// <typeparam name="T">The type / structure of the object to output.</typeparam>
public interface IDocumentReviver<T>
{
    /// <summary>
    /// Unserialize the json to a business instance.
    /// </summary>
    /// <param name="jsonDocument">The json document structured in a business way.</param>
    /// <returns>An instance of the want (T) object</returns>
    T Revive(string jsonDocument);
}
