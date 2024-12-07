namespace CommonInterfaces.DocumentsRelated;

/// <summary>
/// A structure that represents a departure from a train station.
/// </summary>
/// <param name="Destination">The destination (city) of the train.</param>
/// <param name="Via">The list of principal cities that the train passes through.</param>
/// <param name="DepartureMinute">The minute of departure in 0 - 59 format. (The hour is taken from DepartureHour parent)</param>
/// <param name="Train">The train number containing G and L. (Ex. IC5, R1, S30)</param>
/// <param name="Platform">The platform number from which the train departs. (Ex. 13)</param>
/// <param name="Sector">The sector of the train station from which the train departs. (Ex. A, B, AB). Can be null if it's not specific.</param>
/// <param name="UniqueSignsIds"></param>
public record Departure(string Destination, List<string> Via, int DepartureMinute, string? Train, string Platform, string? Sector, List<int> UniqueSignsIds);
