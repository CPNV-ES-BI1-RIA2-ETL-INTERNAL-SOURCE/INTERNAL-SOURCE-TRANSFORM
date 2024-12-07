namespace CommonInterfaces.DocumentsRelated;

/// <summary>
/// A structure that represents a departure hour with a list of departures for that hour.
/// </summary>
/// <param name="HourOfDeparture">Hour of departure in 0 - 23 format.</param>
/// <param name="Departures">A list of departures for that hour.</param>
public record DepartureHour(int HourOfDeparture, List<Departure> Departures);