using BusinessTransformer.Mapping;

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
    /// <throws cref="BusinessTransformerInvalidInputFormatException">Thrown when the input format is invalid based on mapping / schema provided.</throws>
    /// <throws cref="BusinessTransformerFormatException">Thrown when the format of input is invalid. (Like FormatException)</throws>
    dynamic Transform(dynamic input, IEnumerable<FieldMapping<int>> mapping);
}