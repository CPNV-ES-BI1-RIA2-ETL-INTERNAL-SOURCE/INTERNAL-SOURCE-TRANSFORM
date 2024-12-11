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
            !(jsonArray[2] is JsonValue)
        ){
            throw new FormatException();
        }
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