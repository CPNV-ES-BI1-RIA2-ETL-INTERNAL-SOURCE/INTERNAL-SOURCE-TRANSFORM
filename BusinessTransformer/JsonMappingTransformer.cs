using System.Globalization;
using BusinessTransformer.Mapping;
using Newtonsoft.Json.Linq;

namespace BusinessTransformer;

/// <summary>
/// A document transformer that transforms a json document to another json document using a schema mapping.
/// </summary>
public class JsonMappingTransformer(IStringManipulator stringManipulator) : IMappingTransformer
{
    public dynamic Transform(dynamic input, IEnumerable<FieldMapping<int>> mapping)
    {
        var output = new JObject();
        var bag = new Dictionary<string, dynamic>();

        foreach (var fieldMapping in mapping)
        {
            var fromIndex = fieldMapping.From;

            if(input.Count <= fromIndex) throw new BusinessTransformerInvalidInputFormatException("From index out of range");
            var inputValue = input[fromIndex];

            var transformedValue = ApplyMethods(inputValue, fieldMapping.Methods, bag);
            if (!fieldMapping.OnlyBag)
            {
                if (transformedValue == null)
                {
                    output[fieldMapping.Name] = null;
                    continue;
                }
                output[fieldMapping.Name] = JToken.FromObject(transformedValue);
            }
            bag[fieldMapping.Name] = transformedValue;
        }

        return output;
    }

    private dynamic ApplyMethods(dynamic input, IEnumerable<Method> methods, Dictionary<string, dynamic> bag)
    {
        dynamic result = input;

        foreach (var method in methods)
        {
            var parameters = method.ComputedParameters(bag);
            result = method.Name switch
            {
                "RemovePrefixes" => stringManipulator.RemovePrefixes(result.ToString(), parameters.prefixes.ToObject<string[]>(), Convert.ToChar(parameters.separator)),
                "ParseLocalisedDate" => stringManipulator.ParseLocalisedDate(result.ToString(), parameters.format.ToString(), 
                    (parameters.cultures.ToObject<string[]>() as IEnumerable<string>).Select(c => new CultureInfo(c))),
                "Split" => stringManipulator.Split(result.ToString(), parameters.separator.ToString()),
                "ParseHourMinute" => stringManipulator.ParseHourMinute(result.ToString(), parameters.separator.ToString()),
                "Regex" => stringManipulator.ApplyRegex(result.ToString(), parameters.pattern.ToString()),
                "SplitLetterNumber" => stringManipulator.SplitLetterNumber(result.ToString()),
                "Take" => Take(result, parameters.property.ToString()),
                "ProcessArray" => ProcessArray(result, FieldMapping<string>.FromJArray(parameters.fields), FieldMapping<string>.FromJArray(parameters.parentFields), bag),
                "CombineDateTime" => CombineDateTime(result, parameters.dateToAppend.ToObject<DateTime>()),
                "EmptyToNull" => EmptyToNull(result),
                _ => throw new BusinessTransformerMappingException($"Method name '{method.Name}' is not implemented.")
            };
        }

        return result;
    }

    private dynamic EmptyToNull(dynamic result)
    {
        if(!stringManipulator.DoesStringContainsContent(result.ToString())) 
            return null;
        
        //Remove key + value if value is empty on dictionary
        if(result is IDictionary<string, string> enumerable)
        {
            foreach (var (key, value) in enumerable)
            {
                if (!stringManipulator.DoesStringContainsContent(value))
                    result[key] = null;
            }
            return result;
        }
        
        // Remove entry if value is empty in array
        if (result is JArray enumerableArray)
        {
            List<string?> list = new List<string?>();
            foreach (var item in enumerableArray)
            {
                list.Add(stringManipulator.DoesStringContainsContent(item.ToString()) ? item.ToString() : null);
            }
            return list;
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

    private JArray ProcessArray(dynamic inputArray, IEnumerable<FieldMapping<string>> fields, IEnumerable<FieldMapping<string>> parentFields, Dictionary<string, dynamic> bag)
    {
        var resultArray = new JArray();
        
        var objWithParentField = new JObject();
        foreach (var parentField in parentFields)
        {
            var parentFieldValue = bag[parentField.From];
            var transformedValue = ApplyMethods(parentFieldValue, parentField.Methods, bag);
            objWithParentField[parentField.Name] = JToken.FromObject(transformedValue);
        }

        foreach (var item in inputArray)
        {
            var obj = objWithParentField.DeepClone();
            foreach (var field in fields)
            {
                var fieldValue = item[field.From];
                var transformedValue = ApplyMethods(fieldValue, field.Methods, bag);
                obj[field.Name] = JToken.FromObject(transformedValue);
            }
            resultArray.Add(obj);
        }

        return resultArray;
    }
}
