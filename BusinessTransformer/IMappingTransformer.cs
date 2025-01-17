using System.Text.Json.Nodes;

namespace BusinessTransformer;

/// <summary>
/// A generic interface that represents a document transformer.
/// </summary>
public interface IMappingTransformer
{
    /// <summary>
    /// Transforms the input (document like structure) to the output object using a schema mapping.
    /// </summary>
    /// <param name="input">The input document to transform.</param>
    /// <param name="mapping">The mapping schema to use for the transformation (contain rename, processing, etc.).</param>
    /// <returns>The output object transformed.</returns>
    dynamic Transform(dynamic input, dynamic mapping);
}