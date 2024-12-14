using System.Text.Json.Nodes;
using CommonInterfaces.Records.DocumentsRelated;

namespace DocumentParser;

public class DeparturesDocumentReviver: IDocumentReviver<DeparturesDocument>
{
    /// <exception cref="FormatException">if the json is not in the correct business format</exception>
    /// <exception cref="ArgumentNullException">if the json is null</exception>
    /// <exception cref="JsonException">if the json is not valid</exception>
    public DeparturesDocument Revive(string jsonDocument) {
        var jsonArray = JsonNode.Parse(jsonDocument).AsArray();
        CheckFormat(jsonArray);
        
        return new DeparturesDocument(jsonArray[0].ToString(), jsonArray[1].ToString(), ReviveDepartures(jsonArray[2].AsArray()));
    }
    
    /// <summary>
    /// Check that the json is in the correct business format
    /// </summary>
    /// <param name="jsonArray"></param>
    /// <exception cref="FormatException">if the json is not in the correct business format</exception>
    private void CheckFormat(JsonArray jsonArray) {
        if (jsonArray.Count != 3 ||
            !(jsonArray[0] is JsonValue) ||
            !(jsonArray[2] is JsonArray) ||
            !(jsonArray[1] is JsonValue) ||
            !DeparturePropertyExists(jsonArray[2], "Destination")  ||
            !DeparturePropertyExists(jsonArray[2], "Vias")  ||
            !DeparturePropertyExists(jsonArray[2], "Heure de d\u00E9part")  ||
            !DeparturePropertyExists(jsonArray[2], "Ligne")  ||
            !DeparturePropertyExists(jsonArray[2], "Voie")
        ){
            throw new FormatException();
        }
    }

    /// <summary>
    /// Check that the business property exists in the json object
    /// </summary>
    /// <param name="jsonNode"></param>
    /// <param name="propertyName"></param>
    /// <returns>True or false</returns>
    private bool DeparturePropertyExists(JsonNode jsonNode, string propertyName)
    {
        return jsonNode[0].AsObject().ContainsKey(propertyName);
    }
    
    /// <summary>
    /// Instantiate the Departures from json array of them
    /// </summary>
    /// <param name="jsonArray"></param>
    /// <returns>A list of Departure</returns>
    private List<DepartureEntry> ReviveDepartures(JsonArray jsonArray) {
        var Departures = new List<DepartureEntry>();
        
        foreach (var departure in jsonArray) {
            var departureObject = departure.AsObject();
            Departures.Add(new DepartureEntry(
                departureObject["Destination"].ToString(),
                departureObject["Vias"].ToString(),
                departureObject["Heure de d\u00E9part"].ToString(),
                departureObject["Ligne"].ToString(),
                departureObject["Voie"].ToString()
            ));
        }
        
        return Departures;
    }
}