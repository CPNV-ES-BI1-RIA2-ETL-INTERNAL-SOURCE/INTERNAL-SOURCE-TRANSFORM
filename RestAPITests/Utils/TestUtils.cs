﻿using System.Text;
using Newtonsoft.Json;
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
    
    public static TransformRequest CreateInvalidDocumentRequest(string mappingFile)
    {
        return new TransformRequest { Document = new List<string>() { "Invalid document" }, Mapping = GetTestData(mappingFile) };
    }
    
    public static TransformRequest CreateInvalidMappingRequest(string documentFile)
    {
        return new TransformRequest { Document = GetTestRawData(documentFile).Split("\n").ToList(), Mapping = new { } };
    }
    
    public static StringContent SerializeRequestFromFiles(string documentFile, string mappingFile)
    {
        var request = CreateRequestFromFiles(documentFile, mappingFile);  // Use the original method
        var serializedRequest = JsonConvert.SerializeObject(request); // Manually serialize with Newtonsoft.Json
        var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
        return content;
    }

    public static StringContent SerializeInvalidDocumentRequest(string mappingFile)
    {
        var request = CreateInvalidDocumentRequest(mappingFile); // Use the original method
        var serializedRequest = JsonConvert.SerializeObject(request); // Manually serialize with Newtonsoft.Json
        var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
        return content;
    }

    public static StringContent SerializeInvalidMappingRequest(string documentFile)
    {
        var request = CreateInvalidMappingRequest(documentFile); // Use the original method
        var serializedRequest = JsonConvert.SerializeObject(request); // Manually serialize with Newtonsoft.Json
        var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
        return content;
    }
}