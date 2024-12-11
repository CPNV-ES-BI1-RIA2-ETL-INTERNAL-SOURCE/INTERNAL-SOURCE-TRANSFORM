using System.Text.Json;

namespace DocumentParser;
public class DocumentParser: IDocumentParser {
    public string Parse(string rawDocument) {
        var lines = SplitLines(rawDocument);
        var result = ProcessLines(lines);
        return JsonSerializer.Serialize(result);
    }
    
    /// <summary>
    /// Devide the document into lines
    /// </summary>
    /// <param name="rawDocument"></param>
    /// <returns>Each lines of the document in a list</returns>
    private List<string> SplitLines(string rawDocument) {
        return rawDocument
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    /// <summary>
    /// Parse the lines of the document
    /// </summary>
    /// <param name="lines"></param>
    /// <returns>The generated json</returns>
    private List<object> ProcessLines(List<string> lines) {
        var tableParser = new DocumentTableParser();
        var objectsResult = new List<object>();
        
        foreach (var line in lines) {
            // Parse values inside of table
            if (tableParser.InTable() || tableParser.ProcessTableHeaders(line).Any()) {
                tableParser.ProcessLine(line);
                
                // The table is finished
                if (!(tableParser.ProcessTableHeaders(line).Any())) {
                    if (tableParser.TableRows.Any()) objectsResult.Add(tableParser.TableRows);
                    tableParser = new DocumentTableParser();
                    objectsResult.Add(line.Trim());
                }
            // Parse values outside of table
            } else {
                objectsResult.Add(line.Trim());
            }
        }
        // Handle trailing rows if still in a table
        if (tableParser.TableRows.Any()) objectsResult.Add(tableParser.TableRows);
        return objectsResult;
    }
}
