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
    /// Cleans a string by removing leading and trailing characters.
    /// </summary>
    /// <param name="input">The input string to clean.</param>
    /// <param name="charsToTrim">The characters to remove from the beginning and end of the string.</param>
    /// <returns>The cleaned string.</returns>
    string Trim(string input, char[] charsToTrim);
    
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
    (string letter, string number) SplitLetterNumber(string input);
}