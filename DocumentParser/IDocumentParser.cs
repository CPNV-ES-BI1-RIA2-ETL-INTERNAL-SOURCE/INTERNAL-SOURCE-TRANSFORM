using CommonInterfaces.DocumentsRelated;

namespace DocumentParser;

/// <summary>
/// Interface for the DocumentParser
/// </summary>
public interface IDocumentParser
{
    /// <summary>
    /// Parse the input (single string) to returns a formated json.
    /// </summary>
    /// <param name="rawDocument">The input document (as string) with specific patterns (Ex. tables rows are sepparated by min 3 spaces)</param>
    /// <returns>A formated json (key: value)</returns>
    string Parse(string rawDocument);

    /// <summary>
    /// Unserialize the json to a business instance.
    /// </summary>
    /// <param name="jsonDocument">The json document like the returns of the previous Parse method.</param>
    /// <returns>An instance of DeparturesDocument business structure</returns>
    DeparturesDocument Revive(string jsonDocument);
}
