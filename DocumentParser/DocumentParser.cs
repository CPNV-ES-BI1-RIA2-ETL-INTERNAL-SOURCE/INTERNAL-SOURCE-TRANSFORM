using System.Text.Json;
using System.Text.RegularExpressions;
using DeparturesDocument = CommonInterfaces.DocumentsRelated.DeparturesDocument;


namespace DocumentParser;
public class DocumentParser {
    const string tableColumnSeparator = "   ";

    public static string Parse(string rawDocument) {
        var lines = ParseLines(rawDocument);
        var result = ProcessLines(lines);
        return JsonSerializer.Serialize(result);
    }
    
    private static List<string> ParseLines(string rawDocument) {
        return rawDocument
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .ToList();
    }

    private static List<object> ProcessLines(List<string> lines) {
        var result = new List<object>();
        var tableHeaders = new List<string>();
        var tableRows = new List<Dictionary<string, string>>();
        var tableColumnStarts = new List<int>();
        
        foreach (var line in lines) {
            // Detect table header line (as table start) and calculate positions of the columns for all the table lines
            if (!tableHeaders.Any() && line.Contains(tableColumnSeparator)) {
                tableHeaders = line.Split(tableColumnSeparator, StringSplitOptions.RemoveEmptyEntries).Select(header => header.Trim()).ToList();
                tableColumnStarts = tableHeaders.Select(header => line.IndexOf(header, StringComparison.Ordinal)).ToList();
                
            } else if (tableHeaders.Any() && !string.IsNullOrWhiteSpace(line) && line.Contains(tableColumnSeparator)) { // Parse table rows
                var row = ParseDataRow(line, tableHeaders, tableColumnStarts);
                if (row != null) tableRows.Add(row);
                    
            } else if (tableHeaders.Any()){ // End of the table
                if (tableRows.Any()) result.Add(tableRows);
                tableHeaders = new List<string>();
                tableRows = new List<Dictionary<string, string>>();
                result.Add(line.Trim());
                
            } else { // Parse values outside of table
                result.Add(line.Trim());
            }
        }
        // Handle trailing rows if still in a table
        if (tableRows.Any()) result.Add(tableRows);
        
        return result;
    }

    private static Dictionary<string, string> ParseDataRow(string dataLine, List<string> headers, List<int> columnStarts) {
        var row = new Dictionary<string, string>();

        for (var i = 0; i < headers.Count; i++)  {
            var columnStart = columnStarts[i];
            var columnEnd = i < headers.Count - 1 ? columnStarts[i + 1] : dataLine.Length;
            var columnLength = Math.Min(columnEnd - columnStart, dataLine.Length - columnStart);
            // Take the value from column (or empty string if the data is missing/empty)
            row[headers[i]] = (columnStart < dataLine.Length ? dataLine.Substring(columnStart, columnLength).Trim() : "");
        }
        return row;
    }

    public static DeparturesDocument Revive(string rawDocument) {
        throw new NotImplementedException();
    }
}
