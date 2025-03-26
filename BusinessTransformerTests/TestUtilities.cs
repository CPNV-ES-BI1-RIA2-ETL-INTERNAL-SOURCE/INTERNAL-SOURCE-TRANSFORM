using Newtonsoft.Json;

namespace BusinessTransformerTests;

public class TestUtilities
{
    /// <summary>
    /// Get the Deserialized test data from the file (JSON).
    /// </summary>
    /// <param name="fileName">The name of the file to get the test data from. (e.g. /Mapping/Hello.json)</param>
    /// <returns>The deserialized, dynamic test data from the file.</returns>
    public static dynamic GetTestData(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        return JsonConvert.DeserializeObject(File.ReadAllText(path))!;
    }
}