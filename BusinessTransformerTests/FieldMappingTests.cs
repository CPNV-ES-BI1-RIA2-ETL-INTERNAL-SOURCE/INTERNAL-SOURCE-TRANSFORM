using BusinessTransformer;
using BusinessTransformer.Mapping;

namespace BusinessTransformerTests;

public class FieldMappingTests
{
    [Fact]
    public void Transform_InvalidMappingConfigurationFields_ThrowsInvalidMappingException()
    {
        // Given: A valid document to transform and an invalid mapping configuration (missing from and to required fields)
        var testData = TestUtilities.GetTestData("Mapping/InvalidMappingConfigurationFields.json");

        // When: The API is called to parse the invalid mapping
        var exception = Assert.Throws<BusinessTransformerMappingException>(() => FieldMapping<int>.FromJArray(testData));

        // Then: The exception should contain an error message about invalid mappings and the missing fields
        Assert.NotNull(exception);
        var message = exception.Message.ToLower();
        Assert.Contains("from",message);
        Assert.Contains("name", message);
        Assert.Contains("methods", message);
    }
    
    [Fact]
    public void Transform_InvalidMappingConfigurationTypes_ThrowsInvalidMappingException()
    {
        // Given: An invalid mapping configuration ('from' field is not an integer)
        var testData = TestUtilities.GetTestData("Mapping/InvalidMappingTypes.json");

        // When: The API is called to parse the invalid mapping
        var exception = Assert.Throws<BusinessTransformerMappingException>(() => FieldMapping<int>.FromJArray(testData));

        // Then: The exception should contain an error message about invalid mappings types
        Assert.NotNull(exception);
        var message = exception.Message.ToLower();
        Assert.Contains("from",message);
    }
    
    [Fact]
    public void Transform_InvalidMappingConfigurationMethodsType_ThrowsInvalidMappingException()
    {
        // Given: An invalid mapping configuration (methods field is not array)
        var testData = TestUtilities.GetTestData("Mapping/InvalidMappingMethodsType.json");

        // When: The API is called to parse the invalid mapping
        var exception = Assert.Throws<BusinessTransformerMappingException>(() => FieldMapping<int>.FromJArray(testData));

        // Then: The exception should contain an error message about invalid "methods" field type
        Assert.NotNull(exception);
        var message = exception.Message.ToLower();
        Assert.Contains("methods",message);
    }
    
    [Fact]
    public void Transform_InvalidMappingConfigurationMethodNameMissing_ThrowsInvalidMappingException()
    {
        // Given: An invalid mapping configuration (methods field is missing name)
        var testData = TestUtilities.GetTestData("Mapping/InvalidMappingMethodsNameMissing.json");

        // When: The API is called to parse the invalid mapping
        var exception = Assert.Throws<BusinessTransformerMappingException>(() => FieldMapping<int>.FromJArray(testData));

        // Then: The exception should contain an error message about invalid "methods" fields type (missing name)
        Assert.NotNull(exception);
        var message = exception.Message.ToLower();
        Assert.Contains("methods",message);
        Assert.Contains("name",message);
    }
}