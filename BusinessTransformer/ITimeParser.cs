using System.Globalization;

namespace BusinessTransformer;

/// <summary>
/// A generic interface that represents a parser for dates and times from a string.
/// </summary>
public interface ITimeParser
{
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
    (int hour, int minute) ParseHourMinute(string input, string separator);
}