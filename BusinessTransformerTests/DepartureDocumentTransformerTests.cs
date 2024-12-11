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
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)), new List<Departure>());

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: A valid TrainStation object is returned with correctly mapped fields and nested structures
            Assert.IsNotNull(trainStation);
            Assert.That(trainStation.Name, Is.EqualTo("Yverdon-les-Bains"));
            Assert.IsEmpty(trainStation.Departures);
        }
        
        [Test]
        public void Transform_TrainStationNameWithMultiplePrefixes_PrefixesAreRemoved()
        {
            // Given: A DeparturesDocument with a station name containing multiple prefixes
            var departuresDocument = new DeparturesDocument(
                "Bahnhof/Station/Gare de Lausanne", 
                GetFormattedDate(new DateTime(2024, 12, 10)), []);

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
                GetFormattedDate(new DateTime(2024, 12, 10)), []);

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
                []);

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The station name should have the prefix "Stazione di" removed
            Assert.That(trainStation.Name, Is.EqualTo("Locarno"));
        }
        
        [Test]
        public void Transform_InvalidDate_ThrowInvalidArgumentException()
        {
            // Given: A DeparturesDocument with invalid date format
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", "NOT A DATE", new List<Departure>());

            // When: The API is called to transform the parsed document
            // Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument));
        }
        
        [Test]
        public void Transform_TrainStationWithDeparture_DepartureInfoTransformed()
        {
            // Given: A DeparturesDocument for one week with departure tagged with bike sign
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)),
            [
                new Departure("City C", "City A, City B", "09 02", "IC5", "13A")
            ]);

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The specific departure is correctly transformed
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            trainStation.Departures.ForEach(d =>
            {
                Assert.That(d.DepartureStationName, Is.EqualTo("Yverdon-les-Bains"));
                Assert.That(d.DestinationStationName, Is.EqualTo("City C"));
                Assert.That(d.ViaStationNames, Is.EquivalentTo(new List<string>{ "City A", "City B"}));
                Assert.That(d.DepartureTime.Hour, Is.EqualTo(9));
                Assert.That(d.DepartureTime.Minute, Is.EqualTo(2));
                Assert.That(d.DepartureTime.Second, Is.EqualTo(0));
                Assert.That(d.Train, Is.EqualTo(new Train("IC", "5")));
                Assert.That(d.Platform, Is.EqualTo("13"));
                Assert.That(d.Sector, Is.EqualTo("A"));
            });
        }
        
        [Test]
        public void Transform_TrainStationEmptyViaDeparture_ViaListShouldBeEmpty()
        {
            // Given: A DeparturesDocument for one week with departure tagged with bike sign
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)),
            [
                new Departure("City C", " ", "09 02", "IC5", "13A")
            ]);

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The specific departure is correctly transformed
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            Assert.That(trainStation.Departures.First().ViaStationNames, Is.Empty);
        }
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureHourNumber_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)),
            [
                new Departure("City C", "City A, City B", "25 02", "IC5", "13A")
            ]);

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument));
        }
        
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureMinuteNumber_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)),
            [
                new Departure("City C", "City A, City B", "01 60", "IC5", "13A")
            ]);

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument));
        }
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureHour_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", GetFormattedDate(new DateTime(2024, 12, 10)),
            [
                new Departure("City C", "City A, City B", "hello", "IC5", "13A")
            ]);

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument));
        }
        
        [Test]
        public void Transform_MultipleDepartureWithHourAndMinute_DepartureTimeIsCorrectlyFormatted()
        {
            // Given: A departure with DepartureHour parent and minute field
            var documentDate = new DateTime(2024, 10, 12);
            var hours = new List<int> { 12, 13 };
            var minutes = new List<int> { 0, 15, 30, 45 };
            var formattedDateInFrench = GetFormattedDate(documentDate);

            var departureDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", formattedDateInFrench, CreateFakeDepartures(hours, minutes));

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departureDocument);

            // Then: Validate that DepartureTime is correctly represented as DateTime for all combinations
            List<DateTime> departureTimes = GetDateTimeWithIntervals(documentDate, hours, minutes);
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
                []);

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: The TrainStation object should have no departures
            Assert.That(trainStation.Departures, Is.Empty);
        }

        [Test]
        public void Transform_DepartureWithTrainFormatWithoutLine_LTrainValueIsNull()
        {
            // Given: A DeparturesDocument with an unrecognized train format
            var departuresDocument = new DeparturesDocument(
                "Gare de Lausanne", 
                GetFormattedDate(new DateTime(2024, 12, 10)), 
                [
                    new Departure("City Z", "", "10 30", "TGV", "5C")
                ]);

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: Default train values should be used
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures.First();
            Assert.That(departure.Train.G, Is.EqualTo("TGV"));
            Assert.That(departure.Train.L, Is.Null);
        }
        
        [Test]
        public void Transform_DepartureWithSpacedTrainFormat_TrainValuesAreSet()
        {
            // Given: A DeparturesDocument with an unrecognized train format
            var departuresDocument = new DeparturesDocument(
                "Gare de Lausanne", 
                GetFormattedDate(new DateTime(2024, 12, 10)), 
                [
                    new Departure("City Z", "", "10 30", "IC 5", "5C")
                ]);

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: Train values are correct
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures.First();
            Assert.That(departure.Train.G, Is.EqualTo("IC"));
            Assert.That(departure.Train.L, Is.EqualTo("5"));
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", "Départ pour le "+GetFormattedDate(new DateTime(2024, 12, 10)), CreateFakeDepartures([13], [0]));

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: A valid TrainStation object is returned with correct date
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures.First();
            Assert.That(departure.DepartureTime, Is.EqualTo(new DateTime(2024, 12, 10, 13, 0, 0)));
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithEnglishPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", "Departure on 25 February 2024", CreateFakeDepartures([13], [0]));

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument);

            // Then: A valid TrainStation object is returned with correct date
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures.First();
            Assert.That(departure.DepartureTime, Is.EqualTo(new DateTime(2024, 2, 25, 13, 0, 0)));
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithoutDepartures_ShouldTrowFormatException()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = new DeparturesDocument("Gare de Yverdon-les-Bains", "Not a date", new List<Departure>());

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument));
        }
        
        // Helper method to generate formatted dates in French
        private string GetFormattedDate(DateTime date)
        {
            return date.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("fr-FR"));
        }

        // Helper method to generate a list of Departure based on hours
        private List<Departure> CreateFakeDepartures(List<int> hours, List<int> minutes)
        {
            List<Departure> departures = new List<Departure>();
            foreach (var hour in hours)
            {
                foreach (var minute in minutes)
                {
                    departures.Add(new Departure("City C", "City A, City B", hour+" "+minute, "IC 5", "13A"));
                }
            }
            return departures;
        }
        
        // Helper method to validate departure times for a specific date
        private List<DateTime> GetDateTimeWithIntervals(DateTime date, List<int> hours, List<int> minutes)
        {
            List<DateTime> departureTimes = new List<DateTime>();
            foreach (var hour in hours)
            {
                foreach (var minute in minutes)
                {
                    departureTimes.Add(new DateTime(date.Year, date.Month, date.Day, hour, minute, 0));
                }
            }
            return departureTimes;
        }
    }
}
