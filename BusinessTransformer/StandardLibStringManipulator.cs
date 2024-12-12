using System.Globalization;

namespace BusinessTransformer;

/// <summary>
/// An implementation of the string manipulator using the standard .NET library.
/// </summary>
public class StandardLibStringManipulator : IStringManipulator, ITimeParser
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

    public (string letter, string number) SplitLetterNumber(string input)
    {
        string digit = new string(input.Where(char.IsDigit).ToArray());
        string letter = new string(input.Where(char.IsLetter).ToArray());
        return (letter, digit);
    }

    public DateTime ParseLocalisedDate(string input, string format, IEnumerable<CultureInfo> cultures)
    {
        foreach (var culture in cultures)
        {
            if (DateTime.TryParseExact(input, format, culture, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }
        }
        throw new FormatException($"Date string '{input}' is not in a recognized format.");
    }

    public (int hour, int minute) ParseHourMinute(string input, string separator)
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
        return (hour, minute);
    }
}