using System.Text.Json.Nodes;

namespace BusinessTransformer;

/// <summary>
/// A document transformer that transforms a json document to another json document using a schema mapping.
/// </summary>
public class JsonMappingTransformer(IStringManipulator stringManipulator) : IMappingTransformer
{
    private readonly IStringManipulator _stringManipulator = stringManipulator;

    public dynamic Transform(dynamic input, dynamic mapping)
    {
        throw new NotImplementedException();
    }
}