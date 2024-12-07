namespace CommonInterfaces.DocumentsRelated;

/// <summary>
/// A structure that represents a document with departures from a train station.
/// </summary>
/// <param name="RelatedTrainStationName">The train station name that the document is related to (all departures are from this station).</param>
/// <param name="FromDate">The date from which the departures are listed.</param>
/// <param name="ToDate">The date to which the departures are listed.</param>
/// <param name="DepartureHours">A list of hours of departure with a list of departures for each hour.</param>
/// <param name="UniqueSigns">A dictionary of unique signs with their descriptions.</param>
public record DeparturesDocument(string RelatedTrainStationName, string FromDate, string ToDate, List<DepartureHour> DepartureHours, Dictionary<int, string> UniqueSigns);
