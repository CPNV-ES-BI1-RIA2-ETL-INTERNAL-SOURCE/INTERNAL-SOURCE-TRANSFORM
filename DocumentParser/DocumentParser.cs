using System.Text.Json;

namespace DocumentParser;

public class DocumentParser
{
    public static Dictionary<string, object> Parse(string rawDocument)
    {
        throw new NotImplementedException();
    }

    public static Dictionary<string, object> Revive(string rawDocument)
    {
        var parsedDocument = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(rawDocument);
        if (parsedDocument == null || !parsedDocument.ContainsKey("content"))
        {
            throw new ArgumentException("Invalid document structure");
        }

        var content = parsedDocument["content"];
        return CreateDictionary(content);
    }

    private static Dictionary<string, object> CreateDictionary(Dictionary<string, JsonElement> json)
    {
        return json.ToDictionary(
            kvp => kvp.Key,
            kvp => ConvertValue(kvp.Value) ?? kvp.Value.ToString()
        );
    }

    private static object? ConvertValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(property => property.Name, property => ConvertValue(property.Value)),
            JsonValueKind.Array => element.EnumerateArray().Select(ConvertValue).ToList(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt32(out var intValue) => intValue,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
}
