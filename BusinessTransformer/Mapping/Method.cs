using Newtonsoft.Json.Linq;

namespace BusinessTransformer.Mapping;

/// <summary>
/// A method that can be applied to transform a value.
/// </summary>
/// <param name="Name">The name of the method to apply.</param>
/// <param name="Parameters">The parameters to pass to the method.</param>
/// <param name="MetaParameters">The computed parameters to pass to the method (key will be resolved dynamically based on bag).</param>
public record Method(string Name, dynamic Parameters, IEnumerable<MetaParameter> MetaParameters)
{
    /// <summary>
    /// Creates a method from a dynamic object. (Eg. when deserializing from JSON)
    /// </summary>
    /// <param name="obj">The dynamic object to create the method from.</param>
    /// <returns>The method record created.</returns>
    internal static Method FromJObject(dynamic obj)
    {
        if(obj.name == null)
        {
            throw new BusinessTransformerMappingException("Method schema must have 'name' field.");
        }
        var name = obj.name.ToString();
        var parameters = obj.parameters ?? new JObject();
        var metaParameters = MetaParameter.FromJArray(obj.metaParameters ?? new JArray());
        return new Method(name, parameters, metaParameters);
    }
    
    /// <summary>
    /// Creates a list of methods from a dynamic array. (Eg. when deserializing from JSON)
    /// </summary>
    /// <param name="array">The dynamic array to create the methods from.</param>
    /// <returns>The list of methods created.</returns>
    internal static IEnumerable<Method> FromJArray(dynamic array)
    {
        List<Method> methods = new();
        foreach (var method in array)
        {
            methods.Add(FromJObject(method));
        }
        return methods;
    }
    
    /// <summary>
    /// Computes the parameters by replacing the meta parameters with the values from the bag.
    /// </summary>
    /// <param name="bag">The bag containing the values to replace the meta parameters with.</param>
    /// <returns>The computed parameters including the meta parameters replaced.</returns>
    public dynamic ComputedParameters(Dictionary<string, dynamic> bag)
    {
        var computedParameters = Parameters;
        foreach (MetaParameter metaParameter in MetaParameters)
        {
            var metaParameterName = metaParameter.Name;
            var metaParameterValue = metaParameter.Value;
            computedParameters[metaParameterName] = bag[metaParameterValue];
        }
        return computedParameters;
    }
}