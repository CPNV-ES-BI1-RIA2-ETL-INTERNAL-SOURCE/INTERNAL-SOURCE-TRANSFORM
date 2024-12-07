using BusinessTransformer;
using BusinessTransformer.Records;
using CommonInterfaces.DocumentsRelated;
using NUnit.Framework;
using Departure = CommonInterfaces.DocumentsRelated.Departure;

namespace BusinessTransformerTests
{
    public class DepartureDocumentTransformerTests
    {
        private IDocumentTransformer<DeparturesDocument, TrainStation> _transformer;

        [SetUp]
        public void Setup()
        {
            _transformer = new DepartureDocumentTransformer();
        }

        [Test]
        public void Transform_SimpleTrainStationWithoutDepartures_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = new DeparturesDocument("Station A", "2024-10-12", "2024-10-19", new List<DepartureHour>(), new Dictionary<int, string>());

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: A valid TrainStation object is returned with correctly mapped fields and nested structures
            Assert.IsNotNull(trainStation);
            Assert.That(trainStation.Name, Is.EqualTo("Station A"));
            Assert.That(trainStation.City.Name, Is.EqualTo("City A"));
            Assert.IsEmpty(trainStation.Departures);
        }

        [Test]
        public void Transform_TrainStationWithMondaySymbol_DepartureOnlyPresentOnMondays()
        {
            // Given: A DeparturesDocument for one week with departure tagged with '1' sign (only Mondays)
            var departuresDocument = new DeparturesDocument("Station A", "2024-10-12", "2024-10-19", new List<DepartureHour>(), new Dictionary<int, string>
            {
                { 1, "Monday Only" }
            });

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The specific departure is present only on Mondays
            var mondayDepartures = trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Monday).ToList();
            Assert.That(mondayDepartures.Count, Is.EqualTo(1));
        }

        [Test]
        public void Transform_DepartureWithHourAndMinute_DepartureTimeIsCorrectlyFormatted()
        {
            // Given: A departure with DepartureHour parent and minute field
            var startDateTime = new DateTime(2024, 10, 12);
            var endDateTime = new DateTime(2024, 10, 19);
            var hours = new List<int> { 12, 13 };
            var minutes = new List<int> { 0, 15, 30, 45 };
            var formattedStartInFrench = GetFormattedDate(startDateTime);
            var formattedEndInFrench = GetFormattedDate(endDateTime);

            var departureDocument = new DeparturesDocument("Station A", formattedStartInFrench, formattedEndInFrench, 
                CreateDepartureHours(hours), new Dictionary<int, string>());

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departureDocument);

            // Then: Validate that DepartureTime is correctly represented as DateTime for all combinations
            List<DateTime> departureTimes = GetDateTimeWithIntervals(startDateTime, endDateTime, hours, minutes);
            for(int i = 0; i < departureTimes.Count; i++)
            {
                Assert.That(trainStation.Departures[i].DepartureTime, Is.EqualTo(departureTimes[i]));
            }
        }
        
        private DepartureHour CreateFakeDepartureHour(int hour)
        {
            List<int> minutes = new List<int> { 0, 15, 30, 45 };
            List<Departure> departures = minutes.Select(minute => new Departure("City C", new List<string>{ "City A", "City B"}, minute, "IC5", "13", "A", new List<int>())).ToList();
            return new DepartureHour(hour, departures);
        }
        
        // Helper method to generate formatted dates in French
        private string GetFormattedDate(DateTime date)
        {
            return date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("fr-FR"));
        }

        // Helper method to generate a list of DepartureHour
        private List<DepartureHour> CreateDepartureHours(List<int> hours)
        {
            var departureHours = new List<DepartureHour>();
            foreach (var hour in hours)
            {
                departureHours.Add(CreateFakeDepartureHour(hour)); // Assuming CreateFakeDepartureHour is a method to create fake DepartureHour objects
            }
            return departureHours;
        }

        //, List<BusinessTransformer.Records.Departure> departures
        
        // Helper method to validate departure times for a given range of dates and hours
        private List<DateTime> GetDateTimeWithIntervals(DateTime startDateTime, DateTime endDateTime, List<int> hours, List<int> minutes)
        {
            DateTime currentDay = startDateTime;
            List<DateTime> departureTimes = new List<DateTime>();
            while (currentDay <= endDateTime)
            {
                foreach (var hour in hours)
                {
                    foreach (var minute in minutes)
                    {
                        departureTimes.Add(new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, hour, minute, 0));
                    }
                }
                currentDay = currentDay.AddDays(1);
            }
            return departureTimes;
        }

    }
}
