namespace BusinessTransformer.Records;

/// <summary>
/// Represents a train station with its associated city and departures.
/// </summary>
/// <param name="Name">The name of the train station.</param>
/// <param name="Departures">An array of departures from the train station.</param>
/// <param name="City">The city in which the train station is located.</param>
public record TrainStation(string Name, Departure[] Departures, City City);