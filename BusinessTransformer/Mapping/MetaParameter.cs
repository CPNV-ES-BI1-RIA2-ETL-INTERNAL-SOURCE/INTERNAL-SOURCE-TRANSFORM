namespace BusinessTransformer.Mapping;

/// <summary>
/// A meta parameter that can be used to replace a value in the parameters with a value from the bag.
/// </summary>
/// <param name="Name">The name of the parameter to replace (key).</param>
/// <param name="Value">The value to replace the parameter with (value).</param>
public record MetaParameter(string Name, string Value)
{
    /// <summary>
    /// Creates a meta parameter from a dynamic property. (Eg. when deserializing from JSON)
    /// </summary>
    /// <param name="prop">The dynamic property to create the meta parameter from.</param>
    /// <returns>The meta parameter created.</returns>
    internal static MetaParameter FromJProperty(dynamic prop)
    {
        return new MetaParameter(prop.Name, prop.Value.ToString());
    }
    
    /// <summary>
    /// Creates a list of meta parameters from a dynamic array. (Eg. when deserializing from JSON)
    /// </summary>
    /// <param name="array">The dynamic array to create the meta parameters from.</param>
    /// <returns>The list of meta parameters created.</returns>
    internal static IEnumerable<MetaParameter> FromJArray(dynamic array)
    {
        List<MetaParameter> metaParameters = new();
        foreach (var metaParameter in array)
        {
            metaParameters.Add(FromJProperty(metaParameter));
        }
        return metaParameters;
    }
}