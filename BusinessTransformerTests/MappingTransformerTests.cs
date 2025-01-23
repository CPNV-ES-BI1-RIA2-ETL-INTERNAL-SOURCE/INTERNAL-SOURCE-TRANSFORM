using BusinessTransformer;
using BusinessTransformer.Mapping;
using Newtonsoft.Json;

namespace BusinessTransformerTests
{
    public class MappingTransformerTests
    {
        private IMappingTransformer _transformer;
        private dynamic _mapping;

        [SetUp]
        public void Setup()
        {
            // TODO: Move this initialization to a JsonMappingTransformerTests if multiple implementations are added
            _transformer = new JsonMappingTransformer(new StandardLibStringManipulator());
            _mapping = FieldMapping<int>.FromJArray(GetTestData("Mapping.json"));
        }
        
        private static dynamic GetTestData(string fileName)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", fileName);
            return JsonConvert.DeserializeObject(File.ReadAllText(path))!;
        }

        [Test]
        public void Transform_SimpleTrainStationWithoutDepartures_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = GetTestData("SimpleTrainStationWithoutDeparturesInput.json");
            var expectedOutput = GetTestData("SimpleTrainStationWithoutDeparturesOutput.json");
            
            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correctly mapped fields and nested structures
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_TrainStationNameWithMultiplePrefixes_PrefixesAreRemoved()
        {
            // Given: A DeparturesDocument with a station name containing multiple prefixes
            var departuresDocument = GetTestData("TrainStationNameWithMultiplePrefixesInput.json");
            var expectedOutput = GetTestData("TrainStationNameWithMultiplePrefixesOutput.json");
            
            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have all prefixes removed
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_StationNameWithFrenchPrefix_PrefixIsRemoved()
        {
            // Given: A DeparturesDocument with a station name containing the French prefix "Gare de"
            var departuresDocument = GetTestData("StationNameWithFrenchPrefixInput.json");
            var expectedOutput = GetTestData("StationNameWithFrenchPrefixOutput.json");
            
            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have the prefix "Gare de" removed but not based on space (cause station name be a composite name)
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        
        [Test]
        public void Transform_StationNameWithItalianPrefix_PrefixIsRemoved()
        {
            // Given: A DeparturesDocument with a station name containing the Italian prefix "Stazione di"
            var departuresDocument = GetTestData("StationNameWithItalianPrefixInput.json");
            var expectedOutput = GetTestData("StationNameWithItalianPrefixOutput.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have the prefix "Stazione di" removed
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_InvalidDate_ThrowInvalidArgumentException()
        {
            // Given: A DeparturesDocument with invalid date format
            var departuresDocument = GetTestData("InvalidDateInput.json");

            // When: The API is called to transform the parsed document
            // Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_InvalidDepartureHourParts_ThrowInvalidArgumentException()
        {
            // Given: A DeparturesDocument with invalid date format
            var departuresDocument = GetTestData("InvalidDepartureHourParts.json");

            // When: The API is called to transform the parsed document
            // Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_InvalidMinute_ThrowInvalidArgumentException()
        {
            // Given: A DeparturesDocument with invalid date format
            var departuresDocument = GetTestData("InvalidMinute.json");

            // When: The API is called to transform the parsed document
            // Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }

        
        [Test]
        public void Transform_TrainStationWithDeparture_DepartureInfoTransformed()
        {
            // Given: A DeparturesDocument for one week with departure
            var departuresDocument = GetTestData("TrainStationWithDepartureInput.json");
            var expectedOutput = GetTestData("TrainStationWithDepartureOutput.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The specific departure is correctly transformed
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_TrainStationEmptyViaDeparture_ViaListShouldBeEmpty()
        {
            // Given: A DeparturesDocument for one week with departure tagged with bike sign
            var departuresDocument = GetTestData("TrainStationEmptyViaDepartureInput.json");
            var expectedOutput = GetTestData("TrainStationEmptyViaDepartureOutput.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The specific departure is correctly transformed
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureHourNumber_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = GetTestData("TrainStationWithInvalidDepartureHourNumberInput.json");

            // When + Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureMinuteNumber_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = GetTestData("TrainStationWithInvalidDepartureMinuteNumberInput.json");

            // When + Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_TrainStationWithInvalidDepartureHour_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = GetTestData("TrainStationWithInvalidDepartureHourInput.json");

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Test]
        public void Transform_MultipleDepartureWithHourAndMinute_DepartureTimeIsCorrectlyFormatted()
        {
            // Given: A departure with DepartureHour parent and minute field
            var departuresDocument = GetTestData("MultipleDepartureWithHourAndMinuteInput.json");
            var expectedOutput = GetTestData("MultipleDepartureWithHourAndMinuteOutput.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Validate that DepartureTime is correctly represented as DateTime for all combinations
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_EmptyDepartureHours_NoDeparturesInTrainStation()
        {
            // Given: A DeparturesDocument with no departure hours
            var departuresDocument = GetTestData("EmptyDepartureHoursInput.json");
            var expectedOutput = GetTestData("EmptyDepartureHoursOutput.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The TrainStation object should have no departures
            Assert.AreEqual(expectedOutput, trainStation);
        }

        [Test]
        public void Transform_DepartureWithTrainFormatWithoutLine_LTrainValueIsNull()
        {
            // Given: A DeparturesDocument with a train format that does not contain a line (L part)
            var departuresDocument = GetTestData("DepartureWithTrainFormatWithoutLineInput.json");
            var expectedOutput = GetTestData("DepartureWithTrainFormatWithoutLineOutput.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Default train values should be used
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_DepartureWithSpacedTrainFormat_TrainValuesAreSet()
        {
            // Given: A DeparturesDocument with an unrecognized train format
            var departuresDocument = GetTestData("DepartureWithSpacedTrainFormatInput.json");
            var expectedOutput = GetTestData("DepartureWithSpacedTrainFormatOutput.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Train values are correct
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = GetTestData("SimpleTrainStationWithPrefixedDateInput.json");
            var expectedOutput = GetTestData("SimpleTrainStationWithPrefixedDateOutput.json");
            
            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithEnglishPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = GetTestData("SimpleTrainStationWithEnglishPrefixedDateInput.json");
            var expectedOutput = GetTestData("SimpleTrainStationWithEnglishPrefixedDateOutput.json");

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.AreEqual(expectedOutput, trainStation);
        }
        
        [Test]
        public void Transform_SimpleTrainStationWithRandomTextPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = GetTestData("SimpleTrainStationWithRandomTextPrefixedDateInput.json");
            var expectedOutput = GetTestData("SimpleTrainStationWithRandomTextPrefixedDateOutput.json");

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.AreEqual(expectedOutput, trainStation);
        }
    }
}
