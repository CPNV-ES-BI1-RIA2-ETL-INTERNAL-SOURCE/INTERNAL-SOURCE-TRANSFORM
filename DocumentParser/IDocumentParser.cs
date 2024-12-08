namespace DocumentParser;

/// <summary>
/// Interface for the DocumentParser
/// </summary>
/// <typeparam name="TInput">The type of the input document.</typeparam>
/// <typeparam name="TOutput">The type / structure of the object to output.</typeparam>
public interface IDocumentParser<TInput, TOutput>
{
    /// <summary>
    /// Parse the input (json businessless structure) to the output object.
    /// </summary>
    /// <param name="input">The input document (json) to parse (json with a "visual" hierarchical structure).</param>
    /// <returns>An instance of the object with a minimal business structure</returns>
    TOutput Parse(TInput input);

    /// <summary>
    /// Unserialize the input (json businessless structure) to an instance.
    /// </summary>
    /// <param name="input">The input document (json) to parse (json with a "visual" hierarchical structure).</param>
    /// <returns>An instance of the object with no business structure</returns>
    object Revive(TInput input);
}
