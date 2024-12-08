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
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)), GetFormattedDate(new DateTime(2024, 12, 16)), new List<DepartureHour>());

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: A valid TrainStation object is returned with correctly mapped fields and nested structures
            Assert.IsNotNull(trainStation);
            Assert.That(trainStation.Name, Is.EqualTo("Yverdon-les-Bains"));
            Assert.IsEmpty(trainStation.Departures);
        }

        [Test]
        public void Transform_TrainStationWithMondaySymbol_DepartureOnlyPresentOnMondaysAndWednesdays()
        {
            // Given: A DeparturesDocument for one week with departure tagged with '#1' ans '#3' sign (only Mondays and Wednesdays)
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)), GetFormattedDate(new DateTime(2024, 12, 16)), new List<DepartureHour>
            {
                new DepartureHour(10, new List<Departure>
                {
                    new Departure("City C", new List<string>{ "City A", "City B"}, 10, "IC5", "13", "A", new List<string>{ "#1", "#3" }),
                }),
            });

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The specific departure is present only on Mondays
            Assert.That(trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Monday).Count, Is.EqualTo(1));
            Assert.That(trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Tuesday).Count, Is.EqualTo(0));
            Assert.That(trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Wednesday).Count, Is.EqualTo(1));
            Assert.That(trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Thursday).Count, Is.EqualTo(0));
        }
        
        [Test]
        public void Transform_TrainStationWithDeparture_DepartureInfoTransformed()
        {
            // Given: A DeparturesDocument for one week with departure tagged with bike sign
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)), GetFormattedDate(new DateTime(2024, 12, 16)), new List<DepartureHour>
            {
                new DepartureHour(9, new List<Departure>
                {
                    new Departure("City C", new List<string>{ "City A", "City B"}, 02, "IC5", "13", "A", new List<string>{ "#vélo" }),
                }),
            });

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The specific departure is present only on Mondays
            Assert.That(trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Monday).Count, Is.EqualTo(1));
            Assert.That(trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Tuesday).Count, Is.EqualTo(1));
            trainStation.Departures.ForEach(d =>
            {
                Assert.That(d.DepartureStationName, Is.EqualTo("Yverdon-les-Bains"));
                Assert.That(d.ViaStationNames, Is.EquivalentTo(new List<string>{ "City A", "City B"}));
                Assert.That(d.DepartureTime.Hour, Is.EqualTo(9));
                Assert.That(d.DepartureTime.Minute, Is.EqualTo(2));
                Assert.That(d.DepartureTime.Second, Is.EqualTo(0));
                Assert.That(d.Train, Is.EqualTo(new Train("IC", "5")));
                Assert.That(d.Platform, Is.EqualTo("13"));
                Assert.That(d.Sector, Is.EqualTo("A"));
                Assert.That(d.IsBikeReservationRequired, Is.EqualTo(true));
                Assert.That(d.IsNight, Is.EqualTo(false));
            });
        }
        
        [Test]
        public void Transform_TrainStationWithNightDeparture_DepartureInfoTransformed()
        {
            // Given: A DeparturesDocument for one week with departure tagged with night sign
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)), GetFormattedDate(new DateTime(2024, 12, 16)), new List<DepartureHour>
            {
                new DepartureHour(10, new List<Departure>
                {
                    new Departure("City C", new List<string>{ "City A", "City B"}, 59, "R20", "12", "B", new List<string>{ "#nuit" }),
                }),
            });

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The specific departure is present only on Mondays
            Assert.That(trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Monday).Count, Is.EqualTo(1));
            Assert.That(trainStation.Departures.Where(d => d.DepartureTime.DayOfWeek == DayOfWeek.Tuesday).Count, Is.EqualTo(1));
            trainStation.Departures.ForEach(d =>
            {
                Assert.That(d.DestinationStationName, Is.EqualTo("City C"));
                Assert.That(d.ViaStationNames, Is.EquivalentTo(new List<string>{ "City A", "City B"}));
                Assert.That(d.DepartureTime.Hour, Is.EqualTo(10));
                Assert.That(d.DepartureTime.Minute, Is.EqualTo(59));
                Assert.That(d.DepartureTime.Second, Is.EqualTo(0));
                Assert.That(d.Train, Is.EqualTo(new Train("R", "20")));
                Assert.That(d.Platform, Is.EqualTo("12"));
                Assert.That(d.Sector, Is.EqualTo("B"));
                Assert.That(d.IsBikeReservationRequired, Is.EqualTo(false));
                Assert.That(d.IsNight, Is.EqualTo(true));
            });
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

            var departureDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", formattedStartInFrench, formattedEndInFrench, 
                CreateDepartureHours(hours));

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departureDocument);

            // Then: Validate that DepartureTime is correctly represented as DateTime for all combinations
            List<DateTime> departureTimes = GetDateTimeWithIntervals(startDateTime, endDateTime, hours, minutes);
            for(int i = 0; i < departureTimes.Count; i++)
            {
                Assert.That(trainStation.Departures[i].DepartureTime, Is.EqualTo(departureTimes[i]));
            }
        }
        
        [Test]
        public void Transform_EmptyDepartureHours_NoDeparturesInTrainStation()
        {
            // Given: A DeparturesDocument with no departure hours
            var departuresDocument = new DeparturesDocument(
                "Gare de Lausanne", 
                GetFormattedDate(new DateTime(2024, 12, 1)), 
                GetFormattedDate(new DateTime(2024, 12, 7)), 
                new List<DepartureHour>());

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The TrainStation object should have no departures
            Assert.That(trainStation.Departures, Is.Empty);
        }

        [Test]
        public void Transform_DepartureWithMultipleOptionalSpecs_CorrectFlagsAreSet()
        {
            // Given: A DeparturesDocument with a departure containing both night and bike reservation specs
            var departuresDocument = new DeparturesDocument(
                "Gare de Genève", 
                GetFormattedDate(new DateTime(2024, 12, 5)), 
                GetFormattedDate(new DateTime(2024, 12, 5)), 
                new List<DepartureHour>
                {
                    new DepartureHour(22, new List<Departure>
                    {
                        new Departure("City X", new List<string>{ "City Y" }, 45, "IC5", "1", "A", new List<string>{ "#nuit", "#vélo" })
                    })
                });

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: Validate that both night and bike reservation flags are true
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures.First();
            Assert.That(departure.IsNight, Is.True);
            Assert.That(departure.IsBikeReservationRequired, Is.True);
        }

        [Test]
        public void Transform_TrainStationNameWithMultiplePrefixes_PrefixesAreRemoved()
        {
            // Given: A DeparturesDocument with a station name containing multiple prefixes
            var departuresDocument = new DeparturesDocument(
                "Bahnhof/Station/Gare de Lausanne", 
                GetFormattedDate(new DateTime(2024, 12, 10)), 
                GetFormattedDate(new DateTime(2024, 12, 16)), 
                new List<DepartureHour>());

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The station name should have all prefixes removed
            Assert.That(trainStation.Name, Is.EqualTo("Lausanne"));
        }
        
        [Test]
        public void Transform_StationNameWithFrenchPrefix_PrefixIsRemoved()
        {
            // Given: A DeparturesDocument with a station name containing the French prefix "Gare de"
            var departuresDocument = new DeparturesDocument(
                "Gare de Yverdon-Champ Pittet", 
                GetFormattedDate(new DateTime(2024, 12, 10)), 
                GetFormattedDate(new DateTime(2024, 12, 16)), 
                new List<DepartureHour>());

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The station name should have the prefix "Gare de" removed but not based on space (cause station name be a composite name)
            Assert.That(trainStation.Name, Is.EqualTo("Yverdon-Champ Pittet"));
        }
        
        [Test]
        public void Transform_StationNameWithItalianPrefix_PrefixIsRemoved()
        {
            // Given: A DeparturesDocument with a station name containing the Italian prefix "Stazione di"
            var departuresDocument = new DeparturesDocument(
                "Stazione di Locarno", 
                GetFormattedDate(new DateTime(2024, 12, 10)), 
                GetFormattedDate(new DateTime(2024, 12, 16)), 
                new List<DepartureHour>());

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The station name should have the prefix "Stazione di" removed
            Assert.That(trainStation.Name, Is.EqualTo("Locarno"));
        }

        [Test]
        public void Transform_DepartureWithInvalidDateRange_NoDeparturesAreAdded()
        {
            // Given: A DeparturesDocument with an invalid date range (fromDate > toDate)
            var departuresDocument = new DeparturesDocument(
                "Gare de Fribourg", 
                GetFormattedDate(new DateTime(2024, 12, 20)), 
                GetFormattedDate(new DateTime(2024, 12, 15)), 
                new List<DepartureHour>
                {
                    CreateFakeDepartureHour(10)
                });

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The TrainStation object should have no departures
            Assert.That(trainStation.Departures, Is.Empty);
        }

        [Test]
        public void Transform_DepartureWithUnrecognizedTrainFormat_DefaultTrainValuesAreSet()
        {
            // Given: A DeparturesDocument with an unrecognized train format
            var departuresDocument = new DeparturesDocument(
                "Gare de Lausanne", 
                GetFormattedDate(new DateTime(2024, 12, 10)), 
                GetFormattedDate(new DateTime(2024, 12, 10)), 
                new List<DepartureHour>
                {
                    new DepartureHour(15, new List<Departure>
                    {
                        new Departure("City Z", new List<string>(), 30, "UNKNOWN", "5", "C", new List<string>())
                    })
                });

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: Default train values should be used
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures.First();
            Assert.That(departure.Train.G, Is.EqualTo("UNKNOWN"));
            Assert.That(departure.Train.L, Is.Null);
        }

        
        private DepartureHour CreateFakeDepartureHour(int hour)
        {
            List<int> minutes = new List<int> { 0, 15, 30, 45 };
            List<Departure> departures = minutes.Select(minute => new Departure("City C", new List<string>{ "City A", "City B"}, minute, "IC5", "13", "A", new List<string>())).ToList();
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
                departureHours.Add(CreateFakeDepartureHour(hour));
            }
            return departureHours;
        }
        
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
