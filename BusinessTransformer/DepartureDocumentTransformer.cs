using System.Text.RegularExpressions;
using BusinessTransformer.Records;
using CommonInterfaces.DocumentsRelated;
using Departure = BusinessTransformer.Records.Departure;

namespace BusinessTransformer;

/// <summary>
/// A class that transforms a DeparturesDocument into a TrainStation.
/// </summary>
public class DepartureDocumentTransformer : IDocumentTransformer<DeparturesDocument, TrainStation>
{
    /// <summary>
    /// Transforms the input (document like structure) to the output object.
    /// </summary>
    /// <param name="input">The input document to transform (already parsed in a business object format, but keeping the general structure of the document).</param>
    /// <returns>The output object transformed.</returns>
    /// <exception cref="FormatException">If the input dates are not in the correct format.</exception> 
    public TrainStation Transform(DeparturesDocument input)
    {
        DateTime fromDate = DateTime.Parse(input.FromDate);
        DateTime toDate = DateTime.Parse(input.ToDate);
        string stationName = GetStationNameWithoutPrefix(input.RelatedTrainStationName);
        List<Departure> departures = GetDepartures(stationName, fromDate, toDate, input.DepartureHours);
        return new TrainStation(stationName, departures);
    }
    
    /// <summary>
    /// Returns the station name without the prefix.
    /// </summary>
    /// <param name="stationName">The station name to remove the prefix from.</param>
    /// <returns>The station name without the prefix.</returns>
    /// <example>
    /// Bahnhof Zürich Oerlikon -> Zürich Oerlikon
    /// Bahnhof/Station Zürich Flughafen -> Zürich Flughafen
    /// Gare de Yverdon-Champ Pittet -> Yverdon-Champ Pittet
    /// Stazione di Locarno -> Locarno
    /// Gare de Fribourg/Freiburg -> Fribourg/Freiburg
    /// </example>
    private string GetStationNameWithoutPrefix(string stationName)
    {
        string[] prefixes = new string[] { "Bahnhof", "Station", "Gare de", "Stazione di" };
        string transformedStationName = stationName;
        
        //Remove the prefix
        foreach (string prefix in prefixes)
        {
            if (transformedStationName.Contains(prefix))
            {
                return transformedStationName.Substring(transformedStationName.LastIndexOf(prefix, StringComparison.Ordinal) + prefix.Length).Trim();
            }
        }

        //Remove slashes at the beginning if there were multiple prefixes
        while (transformedStationName[0] == '/')
        {
            transformedStationName = transformedStationName.Substring(1);
        }
        
        //Remove the first character (because it is a space)
        return transformedStationName.Substring(1);
    }
    
    /// <summary>
    /// Get the business departures for the given station and date range.
    /// </summary>
    /// <param name = "stationName">The name of the station to get the departures for.</param>
    /// <param name="fromDate">The starting date of the departures hours calendar.</param>
    /// <param name="tomDate">The ending date of the departures hours calendar.</param>
    /// <param name="departureHours">The list of departure hours with their associated departures children.</param>
    /// <returns>A list of business departures for the given station and date range ordered by departure time (complete datetime).</returns>
    private List<Departure> GetDepartures(string stationName, DateTime fromDate, DateTime tomDate, List<DepartureHour> departureHours)
    {
        List<Departure> departures = new List<Departure>();
        
        //Traverse the departure hours and departures tree to get the business departures
        foreach (DepartureHour departureHour in departureHours)
        {
            foreach (CommonInterfaces.DocumentsRelated.Departure departure in departureHour.Departures)
            {
                foreach (DateTime date in GetAllDaysIncluding(fromDate, tomDate))
                {
                    if (IsDepartureValidForDate(date, departure.optionalSpecs))
                    {
                        departures.Add(GetBusinessDeparture(stationName, date, departureHour, departure));
                    }
                }
            }
        }

        return departures.OrderBy(d => d.DepartureTime).ToList();
    }
    
    /// <summary>
    /// Transforms a document departure into a business departure.
    /// Assumes that the departure is valid for the given date.
    /// </summary>
    /// <param name="stationName">The name of the station to get the departures for.</param>
    /// <param name="date">The day of the departure.</param>
    /// <param name="hour">The hour of the departure.</param>
    /// <param name="documentDeparture">The document minute departure to transform.</param>
    /// <returns>The business departure transformed.</returns>
    private Departure GetBusinessDeparture(string stationName, DateTime date, DepartureHour hour, CommonInterfaces.DocumentsRelated.Departure documentDeparture)
    {
        DateTime departureTime = new DateTime(date.Year, date.Month, date.Day, hour.HourOfDeparture, documentDeparture.DepartureMinute, 0);
        Train train = ParseTrain(documentDeparture.Train);
        bool isNightTrain = documentDeparture.optionalSpecs.Contains("#nuit");
        bool isBikeReservationRequired = documentDeparture.optionalSpecs.Contains("#vélo");
        return new Departure(stationName, documentDeparture.Destination, documentDeparture.Via, departureTime, train, documentDeparture.Platform, documentDeparture.Sector, isNightTrain, isBikeReservationRequired);
    }
    
    /// <summary>
    /// Parses the train number and type from the input string.
    /// </summary>
    /// <param name="train">The train number and type in the format IR15, RE2, S30, IC1, S1, SN, SN8, RJX, TGV</param>
    /// <returns>The train number and type parsed.</returns>
    /// <example>
    /// IR15 -> IR 15
    /// RE2 -> RE 2
    /// SN -> SN null
    /// </example>
    private Train ParseTrain(string train)
    {
        Regex regex = new Regex(@"(?<g>[A-Z]+)(?<l>\d+)?");
        Match match = regex.Match(train);
        string g = match.Groups["g"].Value;
        string l = match.Groups["l"].Value;
        return new Train(g, String.IsNullOrEmpty(l) ? null : l);
    }
    
    /// <summary>
    /// Get all days between two dates. (The input dates are included in the result)
    /// </summary>
    /// <param name="fromDate">The starting date.</param>
    /// <param name="toDate">The ending date.</param>
    /// <returns>A list of all days between the two dates.</returns>
    private List<DateTime> GetAllDaysIncluding(DateTime fromDate, DateTime toDate)
    {
        DateTime currentDay = fromDate;
        List<DateTime> days = new List<DateTime>();
        while (currentDay <= toDate)
        {
            days.Add(currentDay);
            currentDay = currentDay.AddDays(1);
        }
        return days;
    }
    
    /// <summary>
    /// Based on the optional specs, checks if the departure is valid for the given date.
    /// </summary>
    /// <param name="date">The date to check the departure for.</param>
    /// <param name="optionalSpecs">The list of optional specs for the departure. (potentially inclusing #1, #2, #3, #4, #5, #6, #7)</param>
    /// <returns>True if the departure is valid for the given date, false otherwise.</returns>
    /// <example>
    /// new DateTime(2025, 1, 1), new List<string> { "#3" } -> true
    /// new DateTime(2025, 1, 1), new List<string> { "#1", "#2" } -> false
    /// new DateTime(2025, 1, 1), new List<string> { } -> true
    /// </example>
    private bool IsDepartureValidForDate(DateTime date, List<string> optionalSpecs)
    {
        Regex regex = new Regex(@"#(?<day>\d+)");
        bool foundOneSpecWithDay = false;
        foreach (string spec in optionalSpecs)
        {
            Match match = regex.Match(spec);
            if (match.Success)
            {
                foundOneSpecWithDay = true;
                int day = int.Parse(match.Groups["day"].Value);
                //TODO : Check for Dimanche
                if (date.DayOfWeek == (DayOfWeek)day)
                {
                    return true;
                }
            }
        }
        return !foundOneSpecWithDay;
    }
}