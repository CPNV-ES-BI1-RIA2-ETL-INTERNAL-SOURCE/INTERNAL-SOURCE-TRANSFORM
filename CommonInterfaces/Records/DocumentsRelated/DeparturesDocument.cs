namespace CommonInterfaces.DocumentsRelated;

/// <summary>
/// A structure that represents a document with departures from a train station.
/// </summary>
/// <param name="RelatedTrainStationName">The train station name that the document is related to (all departures are from this station).</param>
/// <param name="Date">The date from which the departures are listed.</param>
/// <param name="Departure">A list of departures</param>
public record DeparturesDocument(string RelatedTrainStationName, string Date, List<Departure> Departure);
