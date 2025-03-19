﻿using Newtonsoft.Json;
using RestAPI.DTOs;

namespace RestAPITests.Utils;

/// <summary>
/// Static utility class for test methods.
/// Mainly to get test data from files.
/// </summary>
public static class TestUtils
{
    public static string GetTestRawData(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", fileName);
        return File.ReadAllText(path);
    }
    
    public static dynamic GetTestData(string fileName)
    {
        return JsonConvert.DeserializeObject(GetTestRawData(fileName))!;
    }

    public static TransformRequest CreateRequestFromFiles(string documentFile, string mappingFile)
    {
        var document = GetTestRawData(documentFile).Split("\n").ToList();
        var mapping = GetTestData(mappingFile);
        return new TransformRequest { Document = document, Mapping = mapping };
    }
}