using System.Text.Json.Nodes;
using BusinessTransformer;
using Newtonsoft.Json;

namespace BusinessTransformerTests
{
    public class DepartureDocumentTransformerTests
    {
        private IMappingTransformer _transformer;
        private dynamic _mapping;

        [SetUp]
        public void Setup()
        {
            _transformer = new JsonMappingTransformer(new StandardLibStringManipulator());
            _mapping = GetTestData("mapping.json");
        }
        
        private static dynamic GetTestData(string fileName)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", fileName);
            return JsonConvert.DeserializeObject(File.ReadAllText(path));
        }

        [Test]
        public void Transform_SimpleTrainStationWithoutDepartures_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = GetTestData("simple_without_departures.json");

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correctly mapped fields and nested structures
            Assert.IsNotNull(trainStation);
            Assert.That((string)trainStation.Name, Is.EqualTo("Yverdon-les-Bains"));
            Assert.IsEmpty(trainStation.Departures);
        }
        
        [Test]
        public void Transform_TrainStationNameWithMultiplePrefixes_PrefixesAreRemoved()
        {
            // Given: A DeparturesDocument with a station name containing multiple prefixes
            var departuresDocument = GetTestData("simple_multiple_prefixes.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have all prefixes removed
            Assert.That((string)trainStation.Name, Is.EqualTo("Lausanne"));
        }
        
        [Test]
        public void Transform_StationNameWithFrenchPrefix_PrefixIsRemoved()
        {
            // Given: A DeparturesDocument with a station name containing the French prefix "Gare de"
            var departuresDocument = GetTestData("french_prefix.json");
            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have the prefix "Gare de" removed but not based on space (cause station name be a composite name)
            Assert.That((string)trainStation.Name, Is.EqualTo("Yverdon-Champ Pittet"));
        }
        
        
        [Test]
        public void Transform_StationNameWithItalianPrefix_PrefixIsRemoved()
        {
            // Given: A DeparturesDocument with a station name containing the Italian prefix "Stazione di"
            var departuresDocument = GetTestData("italian_prefix.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have the prefix "Stazione di" removed
            Assert.That((string)trainStation.Name, Is.EqualTo("Locarno"));
        }
        
        [Test]
        public void Transform_InvalidDate_ThrowInvalidArgumentException()
        {
            // Given: A DeparturesDocument with invalid date format
            var departuresDocument = GetTestData("invalid_date.json");

            // When: The API is called to transform the parsed document
            // Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_TrainStationWithDeparture_DepartureInfoTransformed()
        {
            // Given: A DeparturesDocument for one week with departure
            var departuresDocument = GetTestData("simple.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The specific departure is correctly transformed
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures[0];
            Assert.That((string)departure.DepartureStationName, Is.EqualTo("Yverdon-les-Bains"));
            Assert.That((string)departure.DestinationStationName, Is.EqualTo("City C"));
            Assert.That((IEnumerable<string>)departure.ViaStationNames, Is.EquivalentTo(new List<string>{ "City A", "City B"}));
            Assert.That(((DateTime)departure.DepartureTime).Hour, Is.EqualTo(9));
            Assert.That(((DateTime)departure.DepartureTime).Minute, Is.EqualTo(2));
            Assert.That(((DateTime)departure.DepartureTime).Second, Is.EqualTo(0));
            Assert.That((string)departure.Train.G, Is.EqualTo("IC"));
            Assert.That((string)departure.Train.L, Is.EqualTo("5"));
            Assert.That((string)departure.Platform, Is.EqualTo("13"));
            Assert.That((string)departure.Sector, Is.EqualTo("A"));
        }
        
        [Test]
        public void Transform_TrainStationEmptyViaDeparture_ViaListShouldBeEmpty()
        {
            // Given: A DeparturesDocument for one week with departure tagged with bike sign
            var departuresDocument = GetTestData("simple_without_vias.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The specific departure is correctly transformed
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            Assert.That(trainStation.Departures[0].ViaStationNames, Is.Empty);
        }
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureHourNumber_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = GetTestData("simple_invalid_departure_hour.json");

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureMinuteNumber_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = GetTestData("simple_invalid_departure_minute.json");

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureHour_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = GetTestData("simple_invalid_departure.json");

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_MultipleDepartureWithHourAndMinute_DepartureTimeIsCorrectlyFormatted()
        {
            // Given: A departure with DepartureHour parent and minute field
            var documentDate = new DateTime(2024, 12, 10);
            var hours = new List<int> { 12, 13 };
            var minutes = new List<int> { 0, 15, 30, 45 };

            var departuresDocument = GetTestData("multiple.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Validate that DepartureTime is correctly represented as DateTime for all combinations
            List<DateTime> departureTimes = GetDateTimeWithIntervals(documentDate, hours, minutes);
            for(int i = 0; i < departureTimes.Count; i++)
            {
                DateTime departureTime = trainStation.Departures[i].DepartureTime;
                Assert.That(departureTime, Is.EqualTo(departureTimes[i]));
            }
        }
        
        [Test]
        public void Transform_EmptyDepartureHours_NoDeparturesInTrainStation()
        {
            // Given: A DeparturesDocument with no departure hours
            var departuresDocument = GetTestData("simple_without_departures.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The TrainStation object should have no departures
            Assert.That(trainStation.Departures, Is.Empty);
        }

        [Test]
        public void Transform_DepartureWithTrainFormatWithoutLine_LTrainValueIsNull()
        {
            // Given: A DeparturesDocument with a train format that does not contain a line (L part)
            var departuresDocument = GetTestData("no-line_no-l.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Default train values should be used
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures[0];
            Assert.That((string)departure.Train.G, Is.EqualTo("TGV"));
            Assert.That((string)departure.Train.L, Is.Null);
        }
        
        [Test]
        public void Transform_DepartureWithSpacedTrainFormat_TrainValuesAreSet()
        {
            // Given: A DeparturesDocument with an unrecognized train format
            var departuresDocument = GetTestData("simple.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Train values are correct
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures[0];
            Assert.That((string)departure.Train.G, Is.EqualTo("IC"));
            Assert.That((string)departure.Train.L, Is.EqualTo("5"));
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = GetTestData("simple.json");
            
            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures[0];
            Assert.That((DateTime)departure.DepartureTime, Is.EqualTo(new DateTime(2024, 12, 10, 9, 2, 0)));
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithEnglishPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = GetTestData("english_prefix.json");

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures[0];
            Assert.That((DateTime)departure.DepartureTime, Is.EqualTo(new DateTime(2024, 2, 25, 13, 0, 0)));
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithRandomTextPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = GetTestData("prefixed_departure.json");

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.That(trainStation.Departures.Count, Is.EqualTo(1));
            var departure = trainStation.Departures[0];
            Assert.That((DateTime)departure.DepartureTime, Is.EqualTo(new DateTime(2024, 12, 12, 13, 0, 0)));
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
