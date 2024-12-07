namespace BusinessTransformer.Records;

/// <summary>
/// Represents a train departure, including details of the journey and train schedule.
/// </summary>
/// <param name="DepartureCity">The city from which the train departs.</param>
/// <param name="DestinationCity">The destination city of the train.</param>
/// <param name="Vias">An array of intermediate cities the train passes through.</param>
/// <param name="DepartureTime">The scheduled departure time of the train.</param>
/// <param name="Train">The train associated with the departure.</param>
/// <param name="Platform">The platform from which the train departs.</param>
/// <param name="Sector">The sector of the station where the train is located.</param>
/// <param name="IsNight">Indicates whether the train operates at night.</param>
/// <param name="IsBikeReservationRequired">Indicates whether a bike reservation is required for the line.</param>
public record Departure(City DepartureCity, City DestinationCity, City[] Vias, DateTime DepartureTime, Train Train, string Platform, string Sector, bool IsNight, bool IsBikeReservationRequired);