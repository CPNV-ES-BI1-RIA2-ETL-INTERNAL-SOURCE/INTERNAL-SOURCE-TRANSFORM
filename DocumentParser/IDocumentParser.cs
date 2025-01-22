namespace DocumentParser;

/// <summary>
/// Interface for the DocumentParser
/// </summary>
public interface IDocumentParser
{
    /// <summary>
    /// Parse the input (single string) to returns a list of dynamic values (strings or dictionaries in case of tables)
    /// </summary>
    /// <param name="lines">The input document (as list of string, representing a document line) with specific patterns (Ex. tables rows are sepparated by min 3 spaces)</param>
    /// <returns>A list of dynamic values</returns>
    List<dynamic> Parse(IEnumerable<string> lines);
}
