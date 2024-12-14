namespace DocumentParser;
public class DocumentTableParser
{
    private const string _tableColumnSeparator = "   ";
    private List<string> _tableHeaders = new List<string>();
    private List<int> _tableColumnStarts = new List<int>();
    public List<Dictionary<string, string>> TableRows { get; private set; } = new List<Dictionary<string, string>>();
    
    /// <summary>
    /// Check if the line is a table header, row or closure and process it accordingly
    /// </summary>
    /// <param name="line"></param>
    public void ProcessLine(string line) 
    {
        // Detect table header line (as table start) and calculate positions of the columns for all the table lines
        if (!_tableHeaders.Any()) {
            DefineHeaders(line);
        // Parse table rows
        } else if (_tableHeaders.Any() && !string.IsNullOrWhiteSpace(line) && line.Contains(_tableColumnSeparator)) {
            ProcessRow(line);
        // End of the table
        } else if (_tableHeaders.Any()){
            _tableHeaders = new List<string>();
        }
    }
    
    /// <summary>
    /// Create a list of strings separated by the table column separator
    /// </summary>
    /// <param name="line"></param>
    /// <returns>The list of table headers</returns>
    public List<string> ProcessTableHeaders(string line)
    {
        return line.Contains(_tableColumnSeparator) ? 
            line.Split(_tableColumnSeparator, StringSplitOptions.RemoveEmptyEntries).Select(header => header.Trim()).ToList():
            new List<string>();
    }
    
    /// <summary>
    /// Check if a table is being processed
    /// </summary>
    /// <returns>Bool</returns>
    public bool InTable()
    {
        return _tableHeaders.Any();
    }
    
    /// <summary>
    /// Set the structure of the table
    /// </summary>
    /// <param name="line"></param>
    private void DefineHeaders(string line)
    {
        _tableHeaders =  ProcessTableHeaders(line);
        _tableColumnStarts = _tableHeaders.Select(header => line.IndexOf(header, StringComparison.Ordinal)).ToList();
    }
    
    /// <summary>
    /// Handle the data of a row and add it to the table
    /// </summary>
    /// <param name="line"></param>
    private void ProcessRow(string line)
    {
        // Ignore the row if it's a 'header reminder'
        if (ProcessTableHeaders(line).SequenceEqual(_tableHeaders)) return;
        var row = ParseDataRow(line);
        if (row != null) TableRows.Add(row);
    }
    
    /// <summary>
    /// Parse the data of a row
    /// </summary>
    /// <param name="dataLine"></param>
    /// <returns>Data of the row as a Dictionary</returns>
    private Dictionary<string, string> ParseDataRow(string line)
    {
        var row = new Dictionary<string, string>();

        for (var i = 0; i < _tableHeaders.Count; i++)  {
            var columnStart = _tableColumnStarts[i];
            var columnEnd = i < _tableHeaders.Count - 1 ? _tableColumnStarts[i + 1] : line.Length;
            var columnLength = Math.Min(columnEnd - columnStart, line.Length - columnStart);
            // Take the value from column (or empty string if the data is missing/empty)
            row[_tableHeaders[i]] = (columnStart < line.Length ? line.Substring(columnStart, columnLength).Trim() : "");
        }
        return row;
    }
}
