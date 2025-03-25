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
        if(obj.from == null || obj.name == null || obj.methods == null)
        {
            throw new BusinessTransformerMappingException("Mapping schema must have 'from', 'name' and 'methods' fields.");
        }
        
        var fromObject = WrapFieldConversionWithExceptionCast<TFrom>(() => obj.from.ToObject<TFrom>(), "from");
        var name = WrapFieldConversionWithExceptionCast<string>(() => obj.name.ToString(), "name");
        var onlyBag = obj.ContainsKey("onlyBag") && WrapFieldConversionWithExceptionCast<bool>(() => obj.onlyBag.ToObject<bool>(), "onlyBag");
        var methods = WrapFieldConversionWithExceptionCast<IEnumerable<Method>>(() => Method.FromJArray(obj.methods), "methods");
        return new FieldMapping<TFrom>(fromObject, name, onlyBag, methods);
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
    
    /// <summary>
    /// Wraps a method call (field conversion) with exception handling and casting.
    /// </summary>
    /// <param name="conversion">The function to convert the field.</param>
    /// <param name="fieldName">The name of the field being converted.</param>
    /// <typeparam name="T">The type of the field to convert to.</typeparam>
    /// <returns>The result of the function if success.</returns>
    /// <exception cref="BusinessTransformerMappingException">This exception is thrown when the conversion of type fails.</exception>
    private static T WrapFieldConversionWithExceptionCast<T>(Func<T> conversion, string fieldName)
    {
        try
        {
            return conversion();
        }
        catch (Exception e)
        {
            if (e is FormatException || e is ArgumentException)
            {
                //This means that one of the fields is not the proper type for the mapping schema (conversion failed).
                throw new BusinessTransformerMappingException($"Invalid mapping field ({fieldName}). {e.Message}", e);
            }
            //Pass through any other exceptions
            throw;
        }
    }
}