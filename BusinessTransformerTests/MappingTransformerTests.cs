using BusinessTransformer;
using BusinessTransformer.Mapping;
using Newtonsoft.Json;

namespace BusinessTransformerTests
{
    public class MappingTransformerTests
    {
        private IMappingTransformer _transformer;
        private dynamic _mapping;

        public MappingTransformerTests()
        {
            // TODO: Move this initialization to a JsonMappingTransformerTests if multiple implementations are added
            _transformer = new JsonMappingTransformer(new StandardLibStringManipulator());
            _mapping = FieldMapping<int>.FromJArray(TestUtilities.GetTestData("Mapping/Mapping.json"));
        }

        [Fact]
        public void Transform_SimpleTrainStationWithoutDepartures_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = TestUtilities.GetTestData("SimpleTrainStationWithoutDeparturesInput.json");
            var expectedOutput = TestUtilities.GetTestData("SimpleTrainStationWithoutDeparturesOutput.json");
            
            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correctly mapped fields and nested structures
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_TrainStationNameWithMultiplePrefixes_PrefixesAreRemoved()
        {
            // Given: A DeparturesDocument with a station name containing multiple prefixes
            var departuresDocument = TestUtilities.GetTestData("TrainStationNameWithMultiplePrefixesInput.json");
            var expectedOutput = TestUtilities.GetTestData("TrainStationNameWithMultiplePrefixesOutput.json");
            
            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have all prefixes removed
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_StationNameWithFrenchPrefix_PrefixIsRemoved()
        {
            // Given: A DeparturesDocument with a station name containing the French prefix "Gare de"
            var departuresDocument = TestUtilities.GetTestData("StationNameWithFrenchPrefixInput.json");
            var expectedOutput = TestUtilities.GetTestData("StationNameWithFrenchPrefixOutput.json");
            
            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have the prefix "Gare de" removed but not based on space (cause station name be a composite name)
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_StationNameWithItalianPrefix_PrefixIsRemoved()
        {
            // Given: A DeparturesDocument with a station name containing the Italian prefix "Stazione di"
            var departuresDocument = TestUtilities.GetTestData("StationNameWithItalianPrefixInput.json");
            var expectedOutput = TestUtilities.GetTestData("StationNameWithItalianPrefixOutput.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The station name should have the prefix "Stazione di" removed
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_InvalidDate_ThrowInvalidArgumentException()
        {
            // Given: A DeparturesDocument with invalid date format
            var departuresDocument = TestUtilities.GetTestData("InvalidDateInput.json");

            // When: The API is called to transform the parsed document
            // Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Fact]
        public void Transform_InvalidDepartureHourParts_ThrowInvalidArgumentException()
        {
            // Given: A DeparturesDocument with invalid date format
            var departuresDocument = TestUtilities.GetTestData("InvalidDepartureHourParts.json");

            // When: The API is called to transform the parsed document
            // Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Fact]
        public void Transform_InvalidMinute_ThrowInvalidArgumentException()
        {
            // Given: A DeparturesDocument with invalid date format
            var departuresDocument = TestUtilities.GetTestData("InvalidMinute.json");

            // When: The API is called to transform the parsed document
            // Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Fact]
        public void Transform_TrainStationWithDeparture_DepartureInfoTransformed()
        {
            // Given: A DeparturesDocument for one week with departure
            var departuresDocument = TestUtilities.GetTestData("TrainStationWithDepartureInput.json");
            var expectedOutput = TestUtilities.GetTestData("TrainStationWithDepartureOutput.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The specific departure is correctly transformed
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_TrainStationEmptyViaDeparture_ViaListShouldBeEmpty()
        {
            // Given: A DeparturesDocument for one week with departure
            var departuresDocument = TestUtilities.GetTestData("TrainStationEmptyViaDepartureInput.json");
            var expectedOutput = TestUtilities.GetTestData("TrainStationEmptyViaDepartureOutput.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The specific departure is correctly transformed
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_TrainStationWithInvalidDepartureHourNumber_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = TestUtilities.GetTestData("TrainStationWithInvalidDepartureHourNumberInput.json");

            // When + Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Fact]
        public void Transform_TrainStationWithInvalidDepartureMinuteNumber_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure minute
            var departuresDocument = TestUtilities.GetTestData("TrainStationWithInvalidDepartureMinuteNumberInput.json");

            // When + Then: An exception is thrown
            Assert.Throws<BusinessTransformerFormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Fact]
        public void Transform_TrainStationWithInvalidDepartureHour_ShouldTrowFormatException()
        {
            // Given: A DeparturesDocument with an invalid departure hour
            var departuresDocument = TestUtilities.GetTestData("TrainStationWithInvalidDepartureHourInput.json");

            // When + Then: An exception is thrown
            Assert.Throws<FormatException>(() => _transformer.Transform(departuresDocument, _mapping));
        }
        
        [Fact]
        public void Transform_MultipleDepartureWithHourAndMinute_DepartureTimeIsCorrectlyFormatted()
        {
            // Given: A departure with DepartureHour parent and minute field
            var departuresDocument = TestUtilities.GetTestData("MultipleDepartureWithHourAndMinuteInput.json");
            var expectedOutput = TestUtilities.GetTestData("MultipleDepartureWithHourAndMinuteOutput.json");

            // When: The transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Validate that DepartureTime is correctly represented as DateTime for all combinations
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_EmptyDepartureHours_NoDeparturesInTrainStation()
        {
            // Given: A DeparturesDocument with no departure hours
            var departuresDocument = TestUtilities.GetTestData("EmptyDepartureHoursInput.json");
            var expectedOutput = TestUtilities.GetTestData("EmptyDepartureHoursOutput.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: The TrainStation object should have no departures
            Assert.Equal(expectedOutput, trainStation);
        }

        [Fact]
        public void Transform_DepartureWithTrainFormatWithoutLine_LTrainValueIsNull()
        {
            // Given: A DeparturesDocument with a train format that does not contain a line (L part)
            var departuresDocument = TestUtilities.GetTestData("DepartureWithTrainFormatWithoutLineInput.json");
            var expectedOutput = TestUtilities.GetTestData("DepartureWithTrainFormatWithoutLineOutput.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Default train values should be used
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_DepartureWithSpacedTrainFormat_TrainValuesAreSet()
        {
            // Given: A DeparturesDocument with an unrecognized train format
            var departuresDocument = TestUtilities.GetTestData("DepartureWithSpacedTrainFormatInput.json");
            var expectedOutput = TestUtilities.GetTestData("DepartureWithSpacedTrainFormatOutput.json");

            // When: Transformation is performed
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: Train values are correct
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_SimpleTrainStationWithPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = TestUtilities.GetTestData("SimpleTrainStationWithPrefixedDateInput.json");
            var expectedOutput = TestUtilities.GetTestData("SimpleTrainStationWithPrefixedDateOutput.json");
            
            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_SimpleTrainStationWithEnglishPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = TestUtilities.GetTestData("SimpleTrainStationWithEnglishPrefixedDateInput.json");
            var expectedOutput = TestUtilities.GetTestData("SimpleTrainStationWithEnglishPrefixedDateOutput.json");

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_SimpleTrainStationWithRandomTextPrefixedDate_InformationIsCorrectlyMapped()
        {
            // Given: A valid DeparturesDocument from the Document Parser
            var departuresDocument = TestUtilities.GetTestData("SimpleTrainStationWithRandomTextPrefixedDateInput.json");
            var expectedOutput = TestUtilities.GetTestData("SimpleTrainStationWithRandomTextPrefixedDateOutput.json");

            // When: The API is called to transform the parsed document
            var trainStation = _transformer.Transform(departuresDocument, _mapping);

            // Then: A valid TrainStation object is returned with correct date
            Assert.Equal(expectedOutput, trainStation);
        }
        
        [Fact]
        public void Transform_EmptyToNullInArray_InformationIsCorrectlyMapped()
        {
            // Given: A array containing empty strings
            _mapping = FieldMapping<int>.FromJArray(TestUtilities.GetTestData("EmptyToNullInArrayMapping.json"));
            var departuresDocument = TestUtilities.GetTestData("EmptyToNullInArrayInput.json");
            var expectedOutput = TestUtilities.GetTestData("EmptyToNullInArrayOutput.json");

            // When: The API is called to transform the parsed document
            var output = _transformer.Transform(departuresDocument, _mapping);

            // Then: A array containing null values instead of empty strings should be returned
            Assert.Equal(expectedOutput, output);
        }
        
        [Fact]
        public void Transform_EmptyToNullInValue_InformationIsCorrectlyMapped()
        {
            // Given: A field containing empty string
            _mapping = FieldMapping<int>.FromJArray(TestUtilities.GetTestData("EmptyToNullInValueMapping.json"));
            var departuresDocument = TestUtilities.GetTestData("EmptyToNullInValueInput.json");
            var expectedOutput = TestUtilities.GetTestData("EmptyToNullInValueOutput.json");

            // When: The API is called to transform the parsed document
            var output = _transformer.Transform(departuresDocument, _mapping);

            // Then: A field containing null value instead of empty string should be returned
            Assert.Equal(expectedOutput, output);
        }
        
        [Fact]
        public void Transform_InvalidMappingMethodName_ThrowsInvalidMappingException()
        {
            // Given: A mapping with invalid method name
            _mapping = FieldMapping<int>.FromJArray(TestUtilities.GetTestData("Mapping/InvalidMappingMethodName.json"));
            var departuresDocument = TestUtilities.GetTestData("TrainStationWithDepartureInput.json");
            
            // When: The API is called to transform the parsed document
            var exception = Assert.Throws<BusinessTransformerMappingException>(() => _transformer.Transform(departuresDocument, _mapping));

            // Then: The exception should contain an error message about invalid mappings and the missing fields
            Assert.NotNull(exception);
            var message = exception.Message.ToLower();
            Assert.Contains("name", message);
            Assert.Contains("methods", message);
        }
        
        [Fact]
        public void Transform_InvalidMappingMethodParameters_ThrowsInvalidMappingException()
        {
            // Given: A mapping with invalid method parameter type
            _mapping = FieldMapping<int>.FromJArray(TestUtilities.GetTestData("Mapping/InvalidMappingMethodParameters.json"));
            var departuresDocument = TestUtilities.GetTestData("TrainStationWithDepartureInput.json");
            
            // When: The API is called to transform the parsed document
            var exception = Assert.Throws<BusinessTransformerMappingException>(() => _transformer.Transform(departuresDocument, _mapping));

            // Then: The exception should contain an error message about invalid mappings and the missing fields
            Assert.NotNull(exception);
            var message = exception.Message.ToLower();
            Assert.Contains("parameters", message);
            Assert.Contains("methods", message);
        }
        
        [Fact]
        public void Transform_InvalidMappingMethodParametersObject_ThrowsInvalidMappingException()
        {
            // Given: A mapping with invalid method parameters object
            _mapping = FieldMapping<int>.FromJArray(TestUtilities.GetTestData("Mapping/InvalidMappingMethodParametersObject.json"));
            var departuresDocument = TestUtilities.GetTestData("TrainStationWithDepartureInput.json");
            
            // When: The API is called to transform the parsed document
            var exception = Assert.Throws<BusinessTransformerMappingException>(() => _transformer.Transform(departuresDocument, _mapping));

            // Then: The exception should contain an error message about invalid mappings and the missing fields
            Assert.NotNull(exception);
            var message = exception.Message.ToLower();
            Assert.Contains("parameters", message);
            Assert.Contains("methods", message);
        }
    }
}
