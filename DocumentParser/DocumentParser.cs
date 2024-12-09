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
        var result = new Dictionary<string, object>();

        foreach (var keyValue in json)
        {
            result[keyValue.Key] = ConvertValue(keyValue) ?? keyValue.Value.ToString();
        }

        return result;
    }

    private static object? ConvertValue(KeyValuePair<string, JsonElement> keyValue)
    {
        var value = keyValue.Value;
        return value.ValueKind switch
        {
            JsonValueKind.Number when value.TryGetInt32(out var intValue) => intValue,
            JsonValueKind.Number when value.TryGetDouble(out var doubleValue) => doubleValue,
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Array => value.EnumerateArray().Select(elem => elem.ValueKind == JsonValueKind.String ? elem.GetString() : elem.ToString()).ToList(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => value
        };
    }
}
