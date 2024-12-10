namespace CommonInterfaces.DocumentsRelated;

/// <summary>
/// A structure that represents a departure from a train station.
/// </summary>
/// <param name="Destination">The destination (city) of the train.</param>
/// <param name="Via">The list of principal cities that the train passes through.</param>
/// <param name="DepartureHour">The hour of departure in 0 00 - 23 59 format. (The hour is taken from DepartureHour parent)</param>
/// <param name="Train">The train number containing G and L. (Ex. IC5, R1, S30)</param>
/// <param name="Platform">The platform number from which the train departs. can optionally include the Sector (Ex. 13 or 13D)</param>
public record Departure(string Destination, List<string> Via, string DepartureHour, string? Train, string Platform);
