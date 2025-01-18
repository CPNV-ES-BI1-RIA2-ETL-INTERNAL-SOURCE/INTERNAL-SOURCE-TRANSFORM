using System.Globalization;

namespace BusinessTransformer;

/// <summary>
/// A generic interface that represents general string manipulations (splitting, cleaning).
/// </summary>
public interface IStringManipulator
{
    /// <summary>
    /// Splits a string by a separator and returns the array of strings.
    /// </summary>
    /// <param name="input">The input string to split.</param>
    /// <param name="separator">The separator (multiple chars) to split the string by.</param>
    /// <returns>The list of strings after splitting. If input is empty, return 0 elements.</returns>
    IEnumerable<string> Split(string input, string separator);
    
    /// <summary>
    /// Does not contain content if the string is null, empty or only with whitespaces.
    /// </summary>
    /// <param name="input">The input string to check.</param>
    /// <returns>False if the string is null, empty or only with whitespaces, true otherwise.</returns>
    bool DoesStringContainsContent(string input);
    
    /// <summary>
    /// Removes prefixes from a string (you can expect trimming).
    /// </summary>
    /// <param name="input">A line of text that may contain prefixes.</param>
    /// <param name="prefixes">A list of prefixes to remove from the input string. You can expect trimming so you don't need to add space after.</param>
    /// <param name="prefixSeparator">The separator between the prefixes and the rest of the string.</param>
    /// <returns>The prefix removed string (if multiple prefixes are found, remove all of them).</returns>
    string RemovePrefixes(string input, IEnumerable<string> prefixes, char prefixSeparator);
    
    /// <summary>
    /// Splits a string into a letter and a number part.
    /// </summary>
    /// <param name="input">The input string to split.</param>
    /// <returns>The letter and number parts of the input string.</returns>
    dynamic SplitLetterNumber(string input);
    
    /// <summary>
    /// Parses a date from a localised string containing a date but can have other text as prefix or suffix.
    /// </summary>
    /// <param name="input">Input string containing a date with a localised format. (Ex. 'Départ le 9 décembre 2024' 'Let's go 9 December 2024')</param>
    /// <param name="format">The format of the date to parse. (Ex. 'd MMMM yyyy' for '9 décembre 2024')</param>
    /// <param name="cultures">The list of cultures to use for parsing the date.</param>
    /// <returns>The date parsed from the input string.</returns>
    /// <exception cref="FormatException">Date does not contain a valid string representation of a date and time.</exception>
    
    DateTime ParseLocalisedDate(string input, string format, IEnumerable<CultureInfo> cultures);
    
    /// <summary>
    /// Parses a string containing hours and minutes.
    /// </summary>
    /// <param name="input">The 'hour minute' string to parse. (Ex. '9 30' or '9:30')</param>
    /// <param name="separator">Separator between hours and minutes. (Ex. ' ' or ':')</param>
    /// <returns>A tuple of hour and minute.</returns>
    /// <exception cref="FormatException">
    /// parts length is not 2.
    /// int.Parse failed.
    /// Hour is not between 0 and 23 (included).
    /// Minute is not between 0 and 59 (included).
    /// </exception>
    /// <exception cref="OverflowException">String represents a number less than Int32.MinValue or greater than Int32.MaxValue.</exception>
    TimeSpan ParseHourMinute(string input, string separator);
}