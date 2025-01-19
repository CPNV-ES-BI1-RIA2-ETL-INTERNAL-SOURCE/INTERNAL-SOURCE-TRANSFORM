using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace BusinessTransformer.mapping;

public record Mapping(dynamic from, string name, bool onlyBag, IEnumerable<Method> methods)
{
    public static Mapping FromJObject(dynamic obj)
    {
        return new Mapping(obj.from, obj.name, obj.ContainsKey("onlyBag") && obj.onlyBag, obj.methods);
    }
}
public record Method(string name, JObject parameters, IEnumerable<JsonProperty> metaParameters);