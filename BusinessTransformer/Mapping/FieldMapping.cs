namespace BusinessTransformer.Mapping;

/// <summary>
/// A mapping schema that represents a transformation from input to output of a property.
/// </summary>
/// <param name="From">The index or name of a property in the input document to transform.</param>
/// <param name="Name">The target name of the property in the output document.</param>
/// <param name="OnlyBag">Whether to store the transformed value in the bag only (hidden from the output).</param>
/// <param name="Methods">List of methods to apply to transform the input value.</param>
public record FieldMapping<T>(T From, string Name, bool OnlyBag, IEnumerable<Method> Methods)
{
    /// <summary>
    /// Creates a mapping schema from a dynamic object. (Eg. when deserializing from JSON)
    /// </summary>
    /// <param name="obj">The dynamic object to create the mapping from.</param>
    /// <returns>The mapping schema created.</returns>
    public static FieldMapping<TFrom> FromJObject<TFrom>(dynamic obj)
    {
        return new FieldMapping<TFrom>(obj.from.ToObject<TFrom>(), obj.name.ToString(), obj.ContainsKey("onlyBag") && (obj.onlyBag.ToObject<bool>()), Method.FromJArray(obj.methods));
    }
    
    /// <summary>
    /// Creates a list of mapping fields from a dynamic array. (Eg. when deserializing from JSON)
    /// </summary>
    /// <param name="mappingSchemaArray">The dynamic array to create the mapping fields from.</param>
    /// <returns>The list of mapping fields created.</returns>
    public static IEnumerable<FieldMapping<T>> FromJArray(dynamic mappingSchemaArray)
    {
        List<FieldMapping<T>> mappings = new();
        foreach (var mapping in mappingSchemaArray)
        {
            mappings.Add(FromJObject<T>(mapping));
        }
        return mappings;
    }
}