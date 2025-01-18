using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace BusinessTransformer;

/// <summary>
/// A document transformer that transforms a json document to another json document using a schema mapping.
/// </summary>
public class JsonMappingTransformer(IStringManipulator stringManipulator) : IMappingTransformer
{
    private readonly IStringManipulator _stringManipulator = stringManipulator;

    public dynamic Transform(dynamic input, dynamic mapping)
    {
        var output = new JObject();
        var bag = new Dictionary<string, dynamic>();

        foreach (var map in mapping)
        {
            var fromIndex = (int)map.from;
            var targetName = (string)map.name;
            var onlyBag = map.ContainsKey("onlyBag") && (bool)map.onlyBag;
            var methods = map.methods as IEnumerable<dynamic>;

            var inputValue = input[fromIndex];
            if (inputValue == null) continue;

            var transformedValue = ApplyMethods(inputValue, methods, bag);
            if (!onlyBag) output[targetName] = JToken.FromObject(transformedValue);
            bag[targetName] = transformedValue;
        }

        return output;
    }

    private dynamic ApplyMethods(dynamic input, IEnumerable<dynamic> methods, dynamic bag)
    {
        dynamic result = input;

        foreach (var method in methods)
        {
            var methodName = (string)method.name;
            var parameters = method.parameters ?? new JObject();
            var metaParameters = method.metaParameters ?? new List<JsonProperty>();
            foreach (var metaParameter in metaParameters)
            {
                var metaParameterName = metaParameter.Name;
                var metaParameterValue = metaParameter.Value.ToString();
                parameters[metaParameterName] = bag[metaParameterValue];
            }

            result = methodName switch
            {
                "RemovePrefixes" => _stringManipulator.RemovePrefixes(result.ToString(), parameters.prefixes.ToObject<string[]>(), Convert.ToChar(parameters.separator)),
                "ParseDate" => _stringManipulator.ParseLocalisedDate(result.ToString(), parameters.format.ToString(), 
                    (parameters.cultures.ToObject<string[]>() as IEnumerable<string>).Select(c => new CultureInfo(c))),
                "Split" => _stringManipulator.Split(result.ToString(), parameters.separator.ToString()),
                "ParseHourMinute" => _stringManipulator.ParseHourMinute(result.ToString(), parameters.separator.ToString()),
                "Regex" => ApplyRegex(result.ToString(), parameters.pattern.ToString()),
                "SplitLetterNumber" => _stringManipulator.SplitLetterNumber(result.ToString()),
                "Take" => Take(result, parameters.property.ToString()),
                "ProcessArray" => ProcessArray(result, parameters.fields, bag),
                "CombineDateTime" => CombineDateTime(result, parameters.dateToAppend.ToObject<DateTime>()),
                _ => throw new NotImplementedException($"Method {methodName} is not implemented.")
            };
        }

        return result;
    }

    private dynamic Take(dynamic result, dynamic toString)
    {
        return result[toString];
    }

    private dynamic CombineDateTime(TimeSpan time, DateTime date)
    {
        return date.Add(time);
    }

    private JArray ProcessArray(dynamic inputArray, dynamic fields, dynamic bag)
    {
        var resultArray = new JArray();

        foreach (var item in inputArray)
        {
            var obj = new JObject();
            foreach (var field in fields)
            {
                var fieldValue = item[field.from.ToString()];
                var transformedValue = ApplyMethods(fieldValue, field.methods, bag);
                obj[field.name.ToString()] = JToken.FromObject(transformedValue);
            }
            resultArray.Add(obj);
        }

        return resultArray;
    }

    private dynamic ApplyRegex(string input, string pattern)
    {
        var match = System.Text.RegularExpressions.Regex.Match(input, pattern);
        Dictionary<string, string> result = new();
        foreach (Group groupName in match.Groups)
        {
            if(groupName is Match) continue;
            result[groupName.Name] = match.Groups[groupName.Name].Value;
        }
        return result;
    }
}
