using System.Globalization;
using System.Text.RegularExpressions;

namespace BusinessTransformer;

/// <summary>
/// An implementation of the string manipulator using the standard .NET library.
/// </summary>
public class StandardLibStringManipulator : IStringManipulator
{
    public IEnumerable<string> Split(string input, string separator)
    {
        if (!DoesStringContainsContent(input))
        {
            return new List<string>();
        }
        return input.Split(separator);
    }
    
    public bool DoesStringContainsContent(string input)
    {
        return !string.IsNullOrWhiteSpace(input.Trim());
    }

    public string RemovePrefixes(string input, IEnumerable<string> prefixes, char prefixSeparator)
    {
        string output = input;
        foreach (var prefix in prefixes)
        {
            if (output.StartsWith(prefix))
            {
                output = output.Substring(prefix.Length);
                if (output.StartsWith(prefixSeparator))
                {
                    output = output.Substring(1);
                }
                output = output.Trim();
            }
        }
        return output;
    }

    public Dictionary<string, string> SplitLetterNumber(string input)
    {
        string digit = new string(input.Where(char.IsDigit).ToArray());
        string letter = new string(input.Where(char.IsLetter).ToArray());
        return new Dictionary<string, string> { { "letter", letter }, { "number", digit } };
    }

    public DateTime ParseLocalisedDate(string input, string format, IEnumerable<CultureInfo> cultures)
    {
        string datePattern = @"\b(\d{1,2}[./]\d{1,2}[./]?\d{2,4}|\d{1,2}\s\w+\s\d{2,4})\b";
        Match match = Regex.Match(input, datePattern);
        string inputWithOnlyDate = match.Value;

        foreach (var culture in cultures)
        {
            if (DateTime.TryParseExact(inputWithOnlyDate, format, culture, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }
        }
        throw new FormatException($"Date string '{input}' is not in a recognized format. Input date detected : '{inputWithOnlyDate}'");
    }

    public TimeSpan ParseHourMinute(string input, string separator)
    {
        var parts = input.Split(separator);
        if (parts.Length != 2)
        {
            throw new FormatException("Input does not contain exactly two parts separated by the separator.");
        }
        int hour = int.Parse(parts[0]);
        int minute = int.Parse(parts[1]);
        if (hour < 0 || hour > 23)
        {
            throw new FormatException("Hour must be between 0 and 23.");
        }
        if (minute < 0 || minute > 59)
        {
            throw new FormatException("Minute must be between 0 and 59.");
        }
        return new TimeSpan(hour, minute, 0);
    }

    public Dictionary<string, string> ApplyRegex(string input, string pattern)
    {
        var match = Regex.Match(input, pattern);
        Dictionary<string, string> result = new();
        foreach (Group groupName in match.Groups)
        {
            if(groupName is Match) continue;
            result[groupName.Name] = match.Groups[groupName.Name].Value;
        }
        return result;
    }
}