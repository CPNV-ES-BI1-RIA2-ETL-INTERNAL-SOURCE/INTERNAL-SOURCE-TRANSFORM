using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        var output = new JsonObject();

        foreach (var map in mapping)
        {
            var fromIndex = (int)map.from;
            var targetName = (string)map.name;
            var onlyBag = map.ContainsKey("onlyBag") && (bool)map.onlyBag;
            var methods = map.methods as IEnumerable<dynamic>;

            var inputValue = input[fromIndex];
            if (inputValue == null) continue;

            var transformedValue = ApplyMethods(inputValue, methods);
            output[targetName] = transformedValue;
        }

        return output;
    }

    private dynamic ApplyMethods(dynamic input, IEnumerable<dynamic> methods)
    {
        dynamic result = input;

        foreach (var method in methods)
        {
            var methodName = (string)method.name;
            var parameters = method.parameters;

            result = methodName switch
            {
                "RemovePrefixes" => _stringManipulator.RemovePrefixes(result.ToString(), parameters.prefixes.ToObject<string[]>(), Convert.ToChar(parameters.separator)),
                "ParseDate" => _stringManipulator.ParseLocalisedDate(result.ToString(), parameters.format.ToString(), 
                    (parameters.cultures.ToObject<string[]>() as IEnumerable<string>).Select(c => new CultureInfo(c))),
                "Split" => _stringManipulator.Split(result.ToString(), parameters.separator.ToString()),
                "ParseHourMinute" => _stringManipulator.ParseHourMinute(result.ToString(), parameters.separator.ToString()),
                "Regex" => ApplyRegex(result.ToString(), parameters.pattern.ToString()),
                "LettersOnly" => _stringManipulator.SplitLetterNumber(result.ToString()).letter,
                "NumbersOnly" => _stringManipulator.SplitLetterNumber(result.ToString()).number,
                _ => throw new NotImplementedException($"Method {methodName} is not implemented.")
            };
        }

        return result;
    }

    private JsonArray ProcessArray(JsonArray inputArray, IEnumerable<dynamic> methods)
    {
        var resultArray = new JsonArray();

        foreach (var item in inputArray)
        {
            var obj = new JsonObject();
            foreach (var field in methods.First().parameters.fields)
            {
                var fieldValue = item[field.from]?.ToString();
                var transformedValue = ApplyMethods(fieldValue, field.methods);
                obj[field.name] = JsonValue.Create(transformedValue);
            }
            resultArray.Add(obj);
        }

        return resultArray;
    }

    private dynamic ApplyRegex(string input, string pattern)
    {
        var match = System.Text.RegularExpressions.Regex.Match(input, pattern);
        return match.Groups;
    }
}
