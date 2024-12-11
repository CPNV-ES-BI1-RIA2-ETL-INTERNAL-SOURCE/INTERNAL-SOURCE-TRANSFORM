using System.Text.Json.Nodes;
using CommonInterfaces.DocumentsRelated;

namespace DocumentParser;

public class DeparturesDocumentReviver: IDocumentReviver<DeparturesDocument>
{
    public DeparturesDocument Revive(string rawDocument) {
        // Instantiate a json document from the raw
        var jsonDocument = new DocumentParser().Parse(rawDocument);
        var jsonArray = JsonNode.Parse(jsonDocument).AsArray();
        
        CheckFormat(jsonArray);
        
        var Departures = ReviveDepartures(jsonArray[1].AsArray());
        return new DeparturesDocument(jsonArray[0].ToString(), jsonArray[2].ToString(), Departures);
    }
    
    private void CheckFormat(JsonArray jsonArray) {
        // Check that the json is in the correct business format
        if (jsonArray.Count != 3 ||
            !(jsonArray[0] is JsonValue) ||
            !(jsonArray[1] is JsonArray) ||
            !(jsonArray[2] is JsonValue) ||
            !DeparturePropertyExists(jsonArray[1], "Destination")  ||
            !DeparturePropertyExists(jsonArray[1], "Vias")  ||
            !DeparturePropertyExists(jsonArray[1], "Heure de d\u00E9part")  ||
            !DeparturePropertyExists(jsonArray[1], "Ligne")  ||
            !DeparturePropertyExists(jsonArray[1], "Voie")
        ){
            throw new FormatException();
        }
    }

    private bool DeparturePropertyExists(JsonNode jsonNode, string propertyName)
    {
        return jsonNode[0].AsObject().ContainsKey(propertyName);
    }
    
    private List<Departure> ReviveDepartures(JsonArray jsonArray) {
        var Departures = new List<Departure>();
        
        foreach (var departure in jsonArray) {
            var departureObject = departure.AsObject();

            Departures.Add(new Departure(
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