namespace BusinessTransformer;

/// <summary>
/// A generic interface that represents a document transformer.
/// </summary>
/// <typeparam name="TInput">The type of the input document.</typeparam>
/// <typeparam name="TOutput">The type / structure of the object to output.</typeparam>
public interface IDocumentTransformer<TInput, TOutput>
{
    /// <summary>
    /// Transforms the input (document like structure) to the output object.
    /// </summary>
    /// <param name="input">The input document to transform (already parsed in a business object format, but keeping the general structure of the document).</param>
    /// <returns>The output object transformed.</returns>
    TOutput Transform(TInput input);
}